using Stockyo.Application.Helper;
using Stockyo.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Interfaces
{
    public interface IBatchService
    {
        Task<Result<BatchDto>> AddBatcheAsync(BatchDto batcheDto,string userId);

        Task<Result<BatchDto>> GetBatcheById(int id,string userId);

        Task<Result<IEnumerable<BatchDto>>> GetBatchesByProductAsync(int storeId, int productId, string userId);
    }
}
