using System.Threading;
using System.Threading.Tasks;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.Entities;
using SettlementBookingSystem.Application.SharedDto;

namespace SettlementBookingSystem.Application.Services
{
    public interface IBookingService
    {
        Task<BussinessResultDto> AddAsync(BookingRequestDto bookingRequestDto, CancellationToken cancellationToken = default);
    }
}
