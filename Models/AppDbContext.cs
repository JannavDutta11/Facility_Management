using Facility_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace Facility_Management.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
<<<<<<< Updated upstream
        public IEnumerable<object> Resources { get; internal set; }

        public DbSet<Resource> Resource { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<ResourceCategory> ResourceCategories { get; set; }
        
        public DbSet<ResourceRule> ResourceRule { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Resource
            modelBuilder.Entity<Resource>()
                .HasKey(r => r.ResourceId);

            // ResourceType
            modelBuilder.Entity<ResourceType>()
                .HasKey(rt => rt.ResourceTypeId);

            // ResourceCategory
            modelBuilder.Entity<ResourceCategory>()
                .HasKey(rc => rc.CategoryId);

            // ResourceRule
            modelBuilder.Entity<ResourceRule>()
                .HasKey(rr => rr.RuleId);

            // Relationships (explicit & safe)
            modelBuilder.Entity<Resource>()
                .HasOne(r => r.ResourceType)
                .WithMany()
                .HasForeignKey(r => r.ResourceTypeId);

            modelBuilder.Entity<Resource>()
                .HasOne(r => r.Category)
                .WithMany()
                .HasForeignKey(r => r.CategoryId);

            modelBuilder.Entity<ResourceRule>()
                .HasOne(rr => rr.Resource)
                .WithMany()
                .HasForeignKey(rr => rr.ResourceId);

            base.OnModelCreating(modelBuilder);
        

        modelBuilder.Entity<Booking>()
                .HasKey(b => b.BookingId);

            modelBuilder.Entity<Booking>()
                .Property(b => b.ResourceId)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.UserId)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.StartTime)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.EndTime)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
               // .HasConversion<int>()
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Booking>()
                .Property(b => b.NumberOfUsers)
                .HasDefaultValue(1);

            
            modelBuilder.Entity<Booking>()
                .ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Booking_Time",
                        "[StartTime] < [EndTime]"
                    );

                    tb.HasCheckConstraint(
                        "CK_Booking_NumberOfUsers",
                        "[NumberOfUsers] > 0"
                    );
                });

        }

=======
        public DbSet<MaintenanceHistory> MaintenanceHistories { get; set; }
>>>>>>> Stashed changes
    }
}