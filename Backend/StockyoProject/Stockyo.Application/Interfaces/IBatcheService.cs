using Stockyo.Application.Helper;
using Stockyo.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Interfaces
{
    public interface IBatcheService
    {
        Task<Result<BatcheDto>> AddBatcheAsync(BatcheDto batcheDto,string userId);

        Task<Result<BatcheDto>> GetBatcheById(int id,string userId);

        Task<Result<IEnumerable<BatcheDto>>> GetBatchesByProductAsync(int storeId, int productId, string userId);
    }
}
