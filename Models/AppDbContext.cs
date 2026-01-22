using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management.Models
{

    // Use IdentityDbContext so AuthController/UserManager/RoleManager work
    public class AppDbContext : IdentityDbContext<ApplicationUser>


    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)

        {

        }

        
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Maintenance> Maintenances { get; set; }
       // public IEnumerable<object> Resources { get; internal set; }
       
        public DbSet<MaintenanceHistory> MaintenanceHistories { get; set; }
        //public IEnumerable<object> Resources { get; internal set; }

        public DbSet<Resource> Resource { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<ResourceCategory> ResourceCategories { get; set; }
        public DbSet<ResourceRule> ResourceRules { get; set; }

        // --- Dev 3 (Utilization Tracking) ---
        public DbSet<UsageLog> UsageLogs { get; set; }
        public DbSet<UsageAudit> UsageAudits { get; set; }


        
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

            // ---------------- UsageLog ----------------
            modelBuilder.Entity<UsageLog>()
                .HasKey(u => u.UsageLogId);

            modelBuilder.Entity<UsageLog>()
                .HasOne(u => u.Booking)
                .WithMany().HasForeignKey(u => u.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsageLog>()
                            .HasIndex(u => new { u.BookingId, u.ActualStartTime, u.ActualEndTime });

            // ---------------- UsageAudit ----------------
            modelBuilder.Entity<UsageAudit>()
                .HasKey(a => a.UsageAuditId);

            modelBuilder.Entity<UsageAudit>()
                           .HasIndex(a => new
                           {
                               a.BookingId,
                               a.ChangedAt
                           });
        }

    }
}


