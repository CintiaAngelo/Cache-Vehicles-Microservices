// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Models;

namespace VehicleManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Garantir que a placa seja única
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.Plate)
                .IsUnique();
        }
    }
}