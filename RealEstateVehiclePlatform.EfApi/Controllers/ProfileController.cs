using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Entities.Concrete;
using RealEstateVehiclePlatform.Entities.DTOs.ProfileDtos;
using System.Security.Claims;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("MyProfile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            return Ok(model);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProfile(
            UpdateUserProfileDto model)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            if (string.IsNullOrWhiteSpace(model.FullName))
                return BadRequest("Ad soyad alanı boş bırakılamaz.");

            if (string.IsNullOrWhiteSpace(model.UserName))
                return BadRequest("Kullanıcı adı boş bırakılamaz.");

            var existingUser = await _userManager.FindByNameAsync(
                model.UserName);

            if (existingUser != null && existingUser.Id != user.Id)
                return BadRequest("Bu kullanıcı adı zaten kullanılıyor.");

            user.FullName = model.FullName.Trim();
            user.UserName = model.UserName.Trim();

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(x => x.Description)
                    .ToList();

                return BadRequest(errors);
            }

            return Ok("Profil başarıyla güncellendi.");
        }
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                return BadRequest("Mevcut şifre alanı zorunludur.");

            if (string.IsNullOrWhiteSpace(model.NewPassword))
                return BadRequest("Yeni şifre alanı zorunludur.");

            if (model.NewPassword.Length < 6)
                return BadRequest("Yeni şifre en az 6 karakter olmalıdır.");

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(x => x.Description)
                    .ToList();

                return BadRequest(errors);
            }

            return Ok("Şifre başarıyla değiştirildi.");
        }
    }
}