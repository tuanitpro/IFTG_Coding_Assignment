using Microsoft.EntityFrameworkCore;
using SettlementBookingSystem.Application.Contexts;

namespace SettlementBookingSystem.Application.UnitTests
{
    public class InMemoryDbContextFactory
    {
        public BookingContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<BookingContext>()
                            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
 
                            .Options;
            var dbContext = new BookingContext(options);

            return dbContext;
        }
    }
}
