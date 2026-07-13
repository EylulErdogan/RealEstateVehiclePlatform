using RealEstateVehiclePlatform.Entities.DTOs.AuthDtos;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);

        Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
    }
}