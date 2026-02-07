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
    public class StoreProfile:Profile
    {
        public StoreProfile()
        {
            CreateMap<StoreDto,Store>().ReverseMap();
        }
    }
}
