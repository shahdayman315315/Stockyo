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
    }
}
