using RealEstateVehiclePlatform.Entities.DTOs.DashboardDtos;

namespace RealEstateVehiclePlatform.Business.Interfaces
{
    public interface IAdminDashboardService
    {
        AdminDashboardSummaryDto GetSummary();
    }
}