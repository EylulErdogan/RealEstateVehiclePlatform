using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;
using RealEstateVehiclePlatform.Entities.DTOs.AuthDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager,
                           IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var exists = await _userManager.FindByEmailAsync(registerDto.Email);

            if (exists != null)
                return "Bu email adresi zaten kayıtlı.";

            var user = new AppUser
            {
                FullName = registerDto.FullName,
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return string.Join(", ", result.Errors.Select(x => x.Description));
            }

            await _userManager.AddToRoleAsync(user, "User");

            return "Kayıt başarılı.";
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                throw new Exception("Email veya şifre hatalı.");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
                throw new Exception("Email veya şifre hatalı.");

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName!),
                new Claim(ClaimTypes.Email,user.Email!)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new TokenResponseDto
            {
                UserId = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? ""
            };
        }
    }
}