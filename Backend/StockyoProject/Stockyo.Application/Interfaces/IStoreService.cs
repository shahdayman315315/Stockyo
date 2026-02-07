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
        Task<Result<StoreDto>> CreateStoreAsync(string userd,StoreDto dto);
    }
}
