using Microsoft.AspNetCore.Identity;
using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;
using RealEstateVehiclePlatform.Entities.DTOs.DashboardDtos;
using RealEstateVehiclePlatform.Entities.Enums;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public AdminDashboardService(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public AdminDashboardSummaryDto GetSummary()
        {
            var listings = _unitOfWork.Listings.GetAll();
            var appointments = _unitOfWork.Appointments.GetAll();

            return new AdminDashboardSummaryDto
            {
                TotalListingCount = listings.Count,

                ActiveListingCount = listings.Count(x => x.Status == ListingStatus.Active),

                PendingListingCount = listings.Count(x => x.Status == ListingStatus.Pending),

                UserCount = _userManager.Users.Count(),

                AppointmentCount = appointments.Count,

                PendingAppointmentCount = appointments.Count(x => x.Status == AppointmentStatus.Pending),

                CategoryCount = _unitOfWork.Categories.GetAll().Count,

                CityCount = _unitOfWork.Cities.GetAll().Count
            };
        }
    }
}