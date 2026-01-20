using Microsoft.EntityFrameworkCore;

namespace Facility_Management.Models
{
    public class AppDbContext : DbContext

    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)

        {

        }


        public DbSet<Booking> Bookings { get; set; }
        public IEnumerable<object> Resources { get; internal set; }

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
        }

    }
}


