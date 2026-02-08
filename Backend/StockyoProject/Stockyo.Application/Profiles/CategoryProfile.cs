using AutoMapper;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;

namespace Stockyo.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Category, CategoryDto>();
        }
    }
}