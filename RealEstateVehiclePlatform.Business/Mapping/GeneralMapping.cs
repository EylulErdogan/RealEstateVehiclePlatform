using AutoMapper;
using RealEstateVehiclePlatform.Entities.Concrete;
using RealEstateVehiclePlatform.Entities.DTOs.AuthDtos;

namespace RealEstateVehiclePlatform.Business.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            // Auth
            CreateMap<RegisterDto, AppUser>();
        }
    }
}