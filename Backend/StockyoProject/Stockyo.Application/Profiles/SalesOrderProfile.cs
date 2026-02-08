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
    public class SalesOrderProfile : Profile
    {
        public SalesOrderProfile()
        {
            // تحويل الفاتورة للعرض
            CreateMap<SalesOrder, SalesOrderResultDto>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SalesOrderItems));

            // تحويل تفاصيل المنتج للعرض
            CreateMap<SalesOrderItem, SalesOrderItemResultDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        }
    }
}
