using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
            var existStore=_unitOfWork.Stores.Query.FirstOrDefault(s=>s.UserId==userId);

            if (existStore is not null)
            {
                return Result<StoreDto>.Failure("User already has a Store");
            }

            var store = _mapper.Map<Store>(dto);
            store.UserId = userId;  

            await _unitOfWork.Stores.AddAsync(store);
            await _unitOfWork.SaveChangesAsync();

            return Result<StoreDto>.Success(dto);
        }
    }
}
