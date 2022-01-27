using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;

namespace DiAnterExpress.Profiles
{
    public class StatusProfile : Profile
    {
        public StatusProfile()
        {
            CreateMap<Shipment, StatusOutputDto>()
            .ForMember(dst => dst.Status,
                opt => opt.MapFrom(src => (Status)src.Status));
        }
    }
}