using Microsoft.AspNetCore.Http;
using Stockyo.Application.Helper;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductDto>> CreateProductAsync(ProductDto dto,string userId);

        Task<Result<BulkProductResultDto>> ImportProductsFromExcelAsync(IFormFile file, int storeId,string userId);
        Task<Result<ProductDto>> GetProductByIdAsync(int productId, string userId);

        Task<Result<PagedResult<ProductDto>>> GetStoreProductsAsync(int storeId, string userId, int pageNumber, int pageSize, string? searchTerm = null);

        Task<Result<bool>> UpdateProductAsync(int productId, ProductDto dto, string userId);
        Task<Result<bool>> DeleteProductAsync(int productId,string userId);

        Task<Result<bool>> BulkDeleteAsync(List<int> productIds, string userId);


    }
}
