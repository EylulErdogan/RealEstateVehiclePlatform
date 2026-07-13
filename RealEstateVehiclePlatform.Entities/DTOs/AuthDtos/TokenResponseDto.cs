namespace RealEstateVehiclePlatform.Entities.DTOs.AuthDtos
{
    public class TokenResponseDto
    {
        public int UserId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}