using Stockyo.Application.Helper; 
using Stockyo.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stockyo.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<CategoryDto>> CreateCategoryAsync(CategoryDto dto, string userId);

        Task<Result<bool>> UpdateCategoryAsync(int id, CategoryDto dto, string userId);

        Task<Result<bool>> DeleteCategoryAsync(int id, string userId);

        Task<Result<CategoryDto>> GetCategoryByIdAsync(int id, string userId);

        Task<Result<PagedResult<CategoryDto>>> GetAllCategoriesAsync(int storeId, string userId, int pageNumber, int pageSize, string? searchTerm = null);


    }
}