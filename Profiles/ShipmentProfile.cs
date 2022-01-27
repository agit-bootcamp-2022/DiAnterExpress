using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Profiles
{
    public class ShipmentProfile : Profile
    {
        public ShipmentProfile()
        {
            CreateMap<Shipment, DtoShipmentOutput>()
            .ForMember(dst => dst.SenderAddress,
                opt => opt.MapFrom(src => new location { Lat = src.SenderAddress.X, Long = src.SenderAddress.Y }))
            .ForMember(dst => dst.ReceiverAddress,
                opt => opt.MapFrom(src => new location { Lat = src.ReceiverAddress.X, Long = src.ReceiverAddress.Y }))
            .ForMember(dst => dst.Status,
                opt => opt.MapFrom(src => (Status)src.Status));
        }
    }
}