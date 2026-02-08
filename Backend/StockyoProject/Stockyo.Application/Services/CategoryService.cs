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
        public async Task<Result<CategoryDto>> CreateCategoryAsync(CategoryDto dto)
        {
          
            var category = _mapper.Map<Category>(dto);
           
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<CategoryDto>.Success(dto);
        }

       
        public async Task<Result<IEnumerable<CategoryDto>>> GetCategoriesByStoreIdAsync(int storeId)
        {

            var categories =  await _unitOfWork.Categories.Query.Where(c => c.StoreId == storeId).ToListAsync();


            var resultDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return Result<IEnumerable<CategoryDto>>.Success(resultDto);
        }
    }
}