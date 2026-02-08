using Stockyo.Application.Helper;
using Stockyo.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Interfaces
{
    public interface ISalesOrderService
    {
   
        Task<Result<SalesOrderResultDto>> CreateSalesOrderAsync(string userId, CreateSalesOrderDto dto);

    
        Task<Result<SalesOrderResultDto>> GetSalesOrderByIdAsync(int id, string userId);

      
        Task<Result<PagedResult<SalesOrderResultDto>>> GetAllSalesOrdersAsync(int storeId, string userId, int pageNumber, int pageSize);

        Task<Result<bool>> DeleteSalesOrderAsync(int id, string userId);


    }
}
