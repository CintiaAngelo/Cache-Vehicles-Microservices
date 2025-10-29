using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;

namespace VehicleManagementAPI.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly AppDbContext _context;
        private readonly ICacheService _cache;

        public VehicleService(AppDbContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            const string cacheKey = "all_vehicles";
            
            var cachedVehicles = await _cache.GetAsync<List<Vehicle>>(cacheKey);
            if (cachedVehicles != null)
            {
                return cachedVehicles;
            }

            var vehicles = await _context.Vehicles.ToListAsync();
            await _cache.SetAsync(cacheKey, vehicles, TimeSpan.FromMinutes(10));
            
            return vehicles;
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            var cacheKey = $"vehicle_{id}";
            
            var cachedVehicle = await _cache.GetAsync<Vehicle>(cacheKey);
            if (cachedVehicle != null)
            {
                return cachedVehicle;
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                await _cache.SetAsync(cacheKey, vehicle, TimeSpan.FromMinutes(10));
            }

            return vehicle;
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            var existingVehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Plate == vehicle.Plate);
            
            if (existingVehicle != null)
            {
                throw new InvalidOperationException("Já existe um veículo com esta placa.");
            }

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            await InvalidateCache();
            return vehicle;
        }

        public async Task<Vehicle?> UpdateVehicleAsync(int id, Vehicle vehicle)
        {
            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null)
                return null;

            var vehicleWithSamePlate = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Plate == vehicle.Plate && v.Id != id);
            
            if (vehicleWithSamePlate != null)
            {
                throw new InvalidOperationException("Já existe outro veículo com esta placa.");
            }

            existingVehicle.Brand = vehicle.Brand;
            existingVehicle.Model = vehicle.Model;
            existingVehicle.Year = vehicle.Year;
            existingVehicle.Plate = vehicle.Plate;

            await _context.SaveChangesAsync();
            await InvalidateCache();

            return existingVehicle;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            await InvalidateCache();

            return true;
        }

        private async Task InvalidateCache()
        {
            var keysToRemove = new List<string> { "all_vehicles" };
            
            for (int i = 1; i <= 1000; i++)
            {
                keysToRemove.Add($"vehicle_{i}");
            }

            foreach (var key in keysToRemove)
            {
                await _cache.RemoveAsync(key);
            }
        }
    }
}