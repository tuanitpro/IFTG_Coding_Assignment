using AutoMapper;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.Entities;

namespace SettlementBookingSystem.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookingRequestDto, Booking>();
        }
    }
}
