using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using GlamourManager.Data.Models;

namespace GlamourManager.Data
{
    public class GlamourDbContext : DbContext
    {
        public GlamourDbContext(DbContextOptions<GlamourDbContext> options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Stylist> Stylists { get; set; } = null!;
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for Price
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Stylist>()
                .HasMany(s => s.Services)
                .WithMany(s => s.Stylists);

            // Seed appointment statuses
            modelBuilder.Entity<AppointmentStatus>().HasData(
                new AppointmentStatus { Id = 1, Name = "Pending" },
                new AppointmentStatus { Id = 2, Name = "Accepted" },
                new AppointmentStatus { Id = 3, Name = "Refused" },
                new AppointmentStatus { Id = 4, Name = "Cancelled" },
                new AppointmentStatus { Id = 5, Name = "Done" }
            );

            // Seed services
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Haircut", Price = 30, DurationMinutes = 30, Description = "Basic haircut service" },
                new Service { Id = 2, Name = "Hair Coloring", Price = 80, DurationMinutes = 120, Description = "Full hair coloring service" },
                new Service { Id = 3, Name = "Styling", Price = 40, DurationMinutes = 45, Description = "Hair styling service" }
            );

            // Seed stylists
            modelBuilder.Entity<Stylist>().HasData(
                new Stylist { Id = 1, Name = "John Smith", Specialization = "Cutting" },
                new Stylist { Id = 2, Name = "Maria Garcia", Specialization = "Coloring" }
            );
        }
    }

    // Design-time DbContext Factory for migrations
    public class GlamourDbContextFactory : IDesignTimeDbContextFactory<GlamourDbContext>
    {
        public GlamourDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GlamourDbContext>();
            optionsBuilder.UseSqlServer("Server=NITRO;Database=Glamour;User Id=sa;Password=.tomAsk08.;TrustServerCertificate=True;");

            return new GlamourDbContext(optionsBuilder.Options);
        }
    }
}