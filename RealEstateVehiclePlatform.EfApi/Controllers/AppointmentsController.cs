using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;
using System.Security.Claims;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost("Create")]
        public IActionResult Create(Appointment appointment)
        {
            try
            {
                var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdValue, out var userId))
                    return Unauthorized("Kullanıcı bilgisi alınamadı.");

                appointment.UserId = userId;

                _appointmentService.Create(appointment);

                return Ok("Randevu başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("MyAppointments")]
        public IActionResult GetMyAppointments()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var values = _appointmentService.GetUserAppointments(userId);

            return Ok(values);
        }

        [HttpGet("ListingAppointments/{listingId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetListingAppointments(int listingId)
        {
            var values = _appointmentService.GetListingAppointments(listingId);

            return Ok(values);
        }

        [HttpPut("Approve/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            try
            {
                _appointmentService.Approve(id);

                return Ok("Randevu onaylandı.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Reject/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id)
        {
            try
            {
                _appointmentService.Reject(id);

                return Ok("Randevu reddedildi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Complete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Complete(int id)
        {
            try
            {
                _appointmentService.Complete(id);

                return Ok("Randevu tamamlandı.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Cancel/{id}")]
        public IActionResult Cancel(int id)
        {
            try
            {
                _appointmentService.Cancel(id);

                return Ok("Randevu iptal edildi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}