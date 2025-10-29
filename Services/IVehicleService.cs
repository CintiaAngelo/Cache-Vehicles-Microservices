using VehicleManagementAPI.Models;

namespace VehicleManagementAPI.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetVehicleByIdAsync(int id);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
        Task<Vehicle?> UpdateVehicleAsync(int id, Vehicle vehicle);
        Task<bool> DeleteVehicleAsync(int id);
    }
}