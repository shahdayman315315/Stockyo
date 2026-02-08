using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class StoreService : IStoreService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreService(UserManager<ApplicationUser> userManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<StoreDto>> CreateStoreAsync(string userId,StoreDto dto)
        {
            var existStore=await _unitOfWork.Stores.Query.FirstOrDefaultAsync(s=>s.UserId==userId);

            if (existStore is not null)
            {
                return Result<StoreDto>.Failure("User already has a Store.");
            }

            var store = _mapper.Map<Store>(dto);
            store.UserId = userId;  

            await _unitOfWork.Stores.AddAsync(store);
            await _unitOfWork.SaveChangesAsync();

            var storeDto=_mapper.Map<StoreDto>(store);    
            return Result<StoreDto>.Success(storeDto);
        }

       
        public async Task<Result<bool>> DeleteStoreAsync(int storeId, string userId)
        {

            var store = await _unitOfWork.Stores.Query
                .FirstOrDefaultAsync(s => s.Id == storeId && s.UserId == userId);

            if (store is null)
                return Result<bool>.Failure("Store not found.");

            //  هل فيه كاتجوري
            var hasCategories = await _unitOfWork.Categories.Query.AnyAsync(c => c.StoreId == storeId);
            if (hasCategories)
                return Result<bool>.Failure("Cannot delete store because it has categories. Delete them first.");
            //  هل فيه برودكتس
            var hasProducts = await _unitOfWork.Products.Query.AnyAsync(p => p.StoreId == storeId);
            if (hasProducts)
                return Result<bool>.Failure("Cannot delete store because it has products. Delete them first.");
            // هل في فواتير 
            var hasSales = await _unitOfWork.SalesOrders.Query.AnyAsync(o => o.StoreId == storeId);
            if (hasSales)
                return Result<bool>.Failure("Cannot delete store because it has sales history.");

            _unitOfWork.Stores.DeleteAsync(store);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<StoreDto>> GetMyStoreAsync(string userId)
        {
            var store = await _unitOfWork.Stores.Query
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (store is null)
            {
                return Result<StoreDto>.Failure("No store found for this user.");
            }
            var dto = _mapper.Map<StoreDto>(store);
            return Result<StoreDto>.Success(dto);
        }

        public async Task<Result<bool>> UpdateStoreAsync(StoreDto dto, string userId)
        {
            var store = await _unitOfWork.Stores.Query
                            .FirstOrDefaultAsync(s => s.Id == dto.Id && s.UserId == userId);

            if (store is null)
                return Result<bool>.Failure("Store not found.");

            store.Name = dto.Name;
            store.Type = dto.Type;

            _unitOfWork.Stores.UpdateAsync(store);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);

        }
    }
}
