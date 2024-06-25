using MediatR;
using SettlementBookingSystem.Application.SharedDto;

namespace SettlementBookingSystem.Application.Bookings.Commands
{
    public class CreateBookingCommand : IRequest<BussinessResultDto>
    {
        public string Name { get; set; }
        public string BookingTime { get; set; }
    }
}
