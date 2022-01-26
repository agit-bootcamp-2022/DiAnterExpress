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
            CreateMap<Shipment, DtoStatus>()
            .ForMember(dst => dst.Status,
                opt => opt.MapFrom(src => (status)src.Status));
        }
        
    }
}