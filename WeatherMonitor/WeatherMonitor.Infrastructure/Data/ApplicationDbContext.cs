using Microsoft.EntityFrameworkCore;
using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<WeatherRecord> WeatherRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherRecord>()
                .HasIndex(w => new { w.City, w.Country })
                .IsUnique();

            modelBuilder.Entity<WeatherRecord>(entity =>
            {
                entity.Property(e => e.Temperature)
                      .HasPrecision(5, 2);

                entity.Property(e => e.MinTemperature)
                      .HasPrecision(5, 2);

                entity.Property(e => e.MaxTemperature)
                      .HasPrecision(5, 2);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
