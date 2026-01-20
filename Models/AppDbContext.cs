using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>

    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)

        {

        }


        public DbSet<Booking> Bookings { get; set; }
        public DbSet<UsageLog> UsageLogs { get; set; }
        public DbSet<UsageAudit> UsageAudits { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
            modelBuilder.Entity<UsageLog>().HasKey(u => u.UsageLogId);

            modelBuilder.Entity<UsageLog>()
                            .HasOne(u => u.Booking)
                            .WithMany()
                            .HasForeignKey(u => u.BookingId)
                            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsageLog>().ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Usage_Time",
                    "([ActualStartTime] IS NULL OR [ActualEndTime] IS NULL OR [ActualStartTime] < [ActualEndTime])"
                );
            });
            modelBuilder.Entity<UsageAudit>().HasKey(a => a.UsageAuditId);


        }

    }
}


