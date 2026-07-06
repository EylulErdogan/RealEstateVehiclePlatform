using Microsoft.AspNetCore.Mvc;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.EfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                _appointmentService.Create(appointment);
                return Ok("Randevu başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("UserAppointments/{userId}")]
        public IActionResult GetUserAppointments(int userId)
        {
            var values = _appointmentService.GetUserAppointments(userId);
            return Ok(values);
        }

        [HttpGet("ListingAppointments/{listingId}")]
        public IActionResult GetListingAppointments(int listingId)
        {
            var values = _appointmentService.GetListingAppointments(listingId);
            return Ok(values);
        }

        [HttpPut("Approve/{id}")]
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