using Stockyo.Application.Helper; 
using Stockyo.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stockyo.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<CategoryDto>> CreateCategoryAsync(CategoryDto dto);
        Task<Result<IEnumerable<CategoryDto>>> GetCategoriesByStoreIdAsync(int storeId);
    }
}