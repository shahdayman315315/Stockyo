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
    public class BatcheService : IBatcheService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BatcheService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            
        }
        public async Task<Result<BatcheDto>> AddBatcheAsync(BatcheDto batcheDto,string userId)
        {
            var existStore =  await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == batcheDto.StoreId && s.UserId == userId);

            if(existStore is null)
            {
                return  Result<BatcheDto>.Failure("Store not found or you don't have permission to add batch");
            }

            var existCategory = await _unitOfWork.Categories.Query.FirstOrDefaultAsync(c => c.StoreId == existStore.Id);

            if(existCategory is null)
            {
                return Result<BatcheDto>.Failure("Category is not found");
            }

            var existProduct=await _unitOfWork.Products.Query.FirstOrDefaultAsync(p=>p.Barcode==batcheDto.Barcode &&p.StoreId==batcheDto.StoreId);

            if(existProduct is null)
            {
                return Result<BatcheDto>.Failure("Product for this Barcode is not found");
            }

            if (batcheDto.ExpiryDate <= batcheDto.ProductionDate)
            {
                return Result<BatcheDto>.Failure("Invalid Expiry and Production Date");
            }

            var batche=_mapper.Map<Batche>(batcheDto);

            batche.ProductId = existProduct.Id;

            await _unitOfWork.Batches.AddAsync(batche);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<BatcheDto>(batche);
            return Result<BatcheDto>.Success(dto);
        }

        public async Task<Result<BatcheDto>> GetBatcheById(int id, string userId)
        {
            var existBatche=await _unitOfWork.Batches.Query.Include(b=>b.Store).FirstOrDefaultAsync(b=>b.Id==id && b.Store.UserId==userId);
        
            if(existBatche is null)
            {
                return Result<BatcheDto>.Failure("Batch not found or user doesn't have permissions to see this batche");
            }

            var dto = _mapper.Map<BatcheDto>(existBatche);

            return Result<BatcheDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<BatcheDto>>> GetBatchesByProductAsync(int storeId, int productId, string userId)
        {
            var existStore = await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

            if(existStore is null)
            {
                return Result<IEnumerable<BatcheDto>>.Failure("Store not found or user doesn't have permissions");
            }

            var existProduct = await _unitOfWork.Products.Query.FirstOrDefaultAsync(p=>p.Id==productId &&p.StoreId==storeId);

            if(existProduct is null)
            {
                return Result<IEnumerable<BatcheDto>>.Failure("Product not found");
            }

            var batches=await _unitOfWork.Batches.Query.OrderByDescending(b => b.ReceivedDate).Where(b=>b.ProductId==productId &&b.StoreId==storeId).ToListAsync();    

            var batchesDtos=_mapper.Map<List<BatcheDto>>(batches);

            return Result<IEnumerable<BatcheDto>>.Success(batchesDtos);
        }
    }
}
