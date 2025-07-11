using Microsoft.EntityFrameworkCore;
using BarberAppointmentSystemF.Models;

namespace BarberAppointmentSystemF.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Appointment konfigürasyonu
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.AppointmentDate);
                entity.HasIndex(e => e.Phone);
                entity.Property(e => e.Status).HasDefaultValue("Pending");
            });

            // User konfigürasyonu
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("Admin");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            
           
        }
    }
}