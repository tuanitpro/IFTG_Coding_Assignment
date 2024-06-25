using Microsoft.EntityFrameworkCore;
using SettlementBookingSystem.Application.Entities;


namespace SettlementBookingSystem.Application.Contexts
{
    public class BookingContext  : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options)
        : base(options)
        {
        }
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryDb");

            base.OnConfiguring(optionsBuilder);
        }

    }
}
