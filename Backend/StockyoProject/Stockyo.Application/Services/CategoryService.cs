using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stockyo.Application.Helper;
using Stockyo.Application.Interfaces;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;
using Stockyo.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stockyo.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<CategoryDto>> CreateCategoryAsync(CategoryDto dto, string userId)
        {
            var store = await _unitOfWork.Stores.Query
                .FirstOrDefaultAsync(s => s.Id == dto.StoreId && s.UserId == userId);

            if (store is null)
            {
                return Result<CategoryDto>.Failure("Store not found.");
            }

            var exists = await _unitOfWork.Categories.Query
                .AnyAsync(c => c.StoreId == dto.StoreId && c.Name == dto.Name);
            if (exists)
            {
                return Result<CategoryDto>.Failure("Category name already exists in this store.");
            }

            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(resultDto);
        }
        public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int id, string userId)
        {
            var category = await _unitOfWork.Categories.Query
                .Include(c => c.Store)
                .FirstOrDefaultAsync(c => c.Id == id && c.Store.UserId == userId);
            if (category is null)
            {
                return Result<CategoryDto>.Failure("Category not found.");
            }

            var dto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(dto);
        }

        public async Task<Result<PagedResult<CategoryDto>>> GetAllCategoriesAsync(int storeId, string userId, int pageNumber, int pageSize, string? searchTerm = null)
        {
            var storeExists = await _unitOfWork.Stores.Query
                .AnyAsync(s => s.Id == storeId && s.UserId == userId);
            if (!storeExists)
            {
                return Result<PagedResult<CategoryDto>>.Failure("Store not found.");
            }

            var query = _unitOfWork.Categories.Query
                .Where(c => c.StoreId == storeId)
                .AsNoTracking();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchTerm)
                                      || c.Description.ToLower().Contains(searchTerm));
            }
            var totalCount = await query.CountAsync();

            var pagedData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<CategoryDto>>(pagedData);

            var result = new PagedResult<CategoryDto>
            {
                Items = dtos,          
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<CategoryDto>>.Success(result);
        }
        public async Task<Result<bool>> UpdateCategoryAsync(int id, CategoryDto dto, string userId)
        {
            var existingCategory = await _unitOfWork.Categories.Query
                .Include(c => c.Store)
                .FirstOrDefaultAsync(c => c.Id == id && c.Store.UserId == userId);

            if (existingCategory is null)
            {
                return Result<bool>.Failure("Category not found.");
            }

            //  نتأكد إن الاسم الجديد مش مستخدم
            var nameExists = await _unitOfWork.Categories.Query
                .AnyAsync(c => c.StoreId == existingCategory.StoreId
                            && c.Name == dto.Name
                            && c.Id != id);

            if (nameExists)
            {
                return Result<bool>.Failure($"Category name '{dto.Name}' is already used.");
            }

            _mapper.Map(dto, existingCategory);
            _unitOfWork.Categories.UpdateAsync(existingCategory);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteCategoryAsync(int id, string userId)
        {
            var category = await _unitOfWork.Categories.Query
                .Include(c => c.Store)
                .FirstOrDefaultAsync(c => c.Id == id && c.Store.UserId == userId);

            if (category is null)
            {
                return Result<bool>.Failure("Category not found.");
            }

            // لو في منتجات ف القسم ده مش هنمسحه
            var hasProducts = await _unitOfWork.Products.Query.AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
            {
                return Result<bool>.Failure("Cannot delete this category because it contains products. Delete products first.");
            }
            _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }






    }
}
