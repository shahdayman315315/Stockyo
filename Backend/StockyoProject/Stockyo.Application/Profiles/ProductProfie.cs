using AutoMapper;
using Stockyo.Domain.DTOs;
using Stockyo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockyo.Application.Profiles
{
    public class ProductProfie:Profile
    {
        public ProductProfie()
        {
            CreateMap<ProductDto,Product>().ReverseMap();
        }
    }
}
