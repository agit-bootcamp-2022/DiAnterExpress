using AutoMapper;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;

namespace DiAnterExpress.Profiles
{
    public class ShipmentTypeProfiles : Profile
    {
        public ShipmentTypeProfiles()
        {
            CreateMap<ShipmentType, DtoShipmentType>();
            CreateMap<ShipmentTypeInsertDto, ShipmentType>();
        }
    }
}
