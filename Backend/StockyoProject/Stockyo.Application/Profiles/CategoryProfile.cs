using AutoMapper;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;

namespace Stockyo.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryDto, Category>().ReverseMap();
        }
    }
}