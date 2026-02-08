using Stockyo.Application.Helper;
using Stockyo.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Interfaces
{
    public interface IStoreService
    {
        Task<Result<StoreDto>> CreateStoreAsync(string userId, StoreDto dto);
        Task<Result<StoreDto>> GetMyStoreAsync(string userId);
        Task<Result<bool>> UpdateStoreAsync(StoreDto dto, string userId);
        Task<Result<bool>> DeleteStoreAsync(int storeId, string userId);

    }
}
