using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stockyo.Application.Helper;
using Stockyo.Application.Interfaces;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;
using Stockyo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Services
{
    public class BatchService : IBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BatchService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        public async Task<Result<BatchDto>> AddBatcheAsync(BatchDto batcheDto,string userId)
        {
            var existStore =  await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == batcheDto.StoreId && s.UserId == userId);

            if(existStore is null)
            {
                return  Result<BatchDto>.Failure("Store not found or you don't have permission to add batch");
            }

            var existCategory = await _unitOfWork.Categories.Query.FirstOrDefaultAsync(c => c.StoreId == existStore.Id);
            if(existCategory is null)
            {
                return Result<BatchDto>.Failure("Category is not found");
            }

            var existProduct=await _unitOfWork.Products.Query.FirstOrDefaultAsync(p=>p.Barcode==batcheDto.Barcode &&p.StoreId==batcheDto.StoreId);
            if(existProduct is null)
            {
                return Result<BatchDto>.Failure("Product for this Barcode is not found");
            }

            if (batcheDto.ExpiryDate <= batcheDto.ProductionDate)
            {
                return Result<BatchDto>.Failure("Invalid Expiry and Production Date");
            }

            var batche=_mapper.Map<Batch>(batcheDto);

            batche.ProductId = existProduct.Id;

            await _unitOfWork.Batches.AddAsync(batche);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<BatchDto>(batche);
            return Result<BatchDto>.Success(dto);
        }

        public async Task<Result<int>> CalculateTotalStockAsync(int storeId, int productId, string userId)
        {
            var existStore = await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

            if (existStore is null)
            {
                return Result<int>.Failure("Store not found or user doesn't have permissions");
            }

            var existProduct = await _unitOfWork.Products.Query.FirstOrDefaultAsync(p => p.Id == productId && p.StoreId == storeId);
        
            if (existProduct is null)
            {
                return Result<int>.Failure("Product not found");
            }

            var productBatches = await _unitOfWork.Batches.Query.Where(b => b.ProductId == productId && b.StoreId == storeId && b.Quantity>0).ToListAsync();

            if(productBatches is null || !productBatches.Any())
            {
                return Result<int>.Success(0);
            }

            var totalStock = productBatches.Sum(b => b.Quantity);

            return Result<int>.Success(totalStock);
        }

        public async Task<Result<bool>> DeleteBatchAsync(int id, string userId)
        {
            var existBatche = await _unitOfWork.Batches.Query.Include(b => b.Store).FirstOrDefaultAsync(b => b.Id == id && b.Store.UserId == userId);

            if (existBatche is null)
            {
                return Result<bool>.Failure("Batch not found or user doesn't have permissions to see this batche");
            }

                _unitOfWork.Batches.DeleteAsync(existBatche);
                await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<BatchDto>> GetBatcheById(int id, string userId)
        {
            var existBatche=await _unitOfWork.Batches.Query.Include(b=>b.Store).FirstOrDefaultAsync(b=>b.Id==id && b.Store.UserId==userId);
        
            if(existBatche is null)
            {
                return Result<BatchDto>.Failure("Batch not found or user doesn't have permissions to see this batche");
            }

            var dto = _mapper.Map<BatchDto>(existBatche);

            return Result<BatchDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<BatchDto>>> GetBatchesByProductAsync(int storeId, int productId, string userId)
        {
            var existStore = await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

            if(existStore is null)
            {
                return Result<IEnumerable<BatchDto>>.Failure("Store not found or user doesn't have permissions");
            }

            var existProduct = await _unitOfWork.Products.Query.FirstOrDefaultAsync(p=>p.Id==productId &&p.StoreId==storeId);

            if(existProduct is null)
            {
                return Result<IEnumerable<BatchDto>>.Failure("Product not found");
            }

            var batches=await _unitOfWork.Batches.Query.OrderByDescending(b => b.ReceivedDate).Where(b=>b.ProductId==productId &&b.StoreId==storeId).ToListAsync();    

            var batchesDtos=_mapper.Map<List<BatchDto>>(batches);

            return Result<IEnumerable<BatchDto>>.Success(batchesDtos);
        }

        public async Task<Result<IEnumerable<ProductNearingExpiryDto>>> GetProductsNearingExpiryDateAsync(int storeId, string userId, int days = 30)
        {
            var existStore = await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

            if(existStore is null)
            {
                return Result<IEnumerable<ProductNearingExpiryDto>>.Failure("Store not found or user doesn't have permissions");
            }

             var currentDate = DateTime.UtcNow;
             var targetDate = currentDate.AddDays(days);

            var productsNearingExpiry = await _unitOfWork.Batches.Query
                .Include(b => b.Product)
                .Where(b => b.StoreId == storeId &&b.Quantity>0 && b.ExpiryDate >= currentDate && b.ExpiryDate <= targetDate )
                .Select(b => new ProductNearingExpiryDto
                {
                    ProductName = b.Product.Name,
                    ExpiryDate = DateOnly.FromDateTime(b.ExpiryDate),
                    Quantity = b.Quantity
                })
                .ToListAsync();

            if(productsNearingExpiry is null || !productsNearingExpiry.Any())
            {
                return Result<IEnumerable<ProductNearingExpiryDto>>.Success(new List<ProductNearingExpiryDto>());
            }

           return Result<IEnumerable<ProductNearingExpiryDto>>.Success(productsNearingExpiry);
        }

        public async Task<Result<BatchDto>> UpdateBatchAsync(int id, BatchDto batchDto, string userId)
        {
            var batch =await _unitOfWork.Batches.Query.Include(b=>b.Store).FirstOrDefaultAsync(b=>b.Id==id && b.Store.UserId==userId);

            if(batch is null)
            {
                return Result<BatchDto>.Failure("Batch not found or user doesn't have permissions to update this batch");
            }

            if(batchDto.ExpiryDate <= batchDto.ProductionDate)
            {
                return Result<BatchDto>.Failure("Invalid Expiry and Production Date");
            }

            _mapper.Map(batchDto,batch);

           // var product = await _unitOfWork.Products.Query.FirstOrDefaultAsync(p => p.Barcode == batchDto.Barcode && p.StoreId == batchDto.StoreId);
           // batch.ProductId = product.Id;

            _unitOfWork.Batches.UpdateAsync(batch); 
             await _unitOfWork.SaveChangesAsync();

            return Result<BatchDto>.Success(batchDto);
        }
    }
}
