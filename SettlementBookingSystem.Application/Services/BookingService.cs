using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.Contexts;
using SettlementBookingSystem.Application.Entities;
using SettlementBookingSystem.Application.Helpers;
using SettlementBookingSystem.Application.SharedDto;

namespace SettlementBookingSystem.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingContext _dbContext;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public BookingService(BookingContext dbContext, IMapper mapper, ILogger<BookingService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;

            _logger = logger;
        }

        public async Task<BussinessResultDto> AddAsync(BookingRequestDto bookingRequestDto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Create new booking Name: {Name} BookingTime: {BookingTime}", bookingRequestDto.Name, bookingRequestDto.BookingTime);
            var booking = _mapper.Map<Booking>(bookingRequestDto);

            booking = booking.ToBookingWithTimeRange(bookingRequestDto.BookingTime);

            var checkInTimeRange = booking.IsBetweenTimeRange(new TimeSpan(09, 00, 00), new TimeSpan(16, 00, 00));
            if (!checkInTimeRange)
            {
                _logger.LogWarning("Out of hours. Time allowed: 09:00 -> 16:00");
                return new BussinessResultDto
                {
                    ErrorCode = BusinessErrorCode.InvalidData,
                    Message = "Out of hours. Time allowed: 09:00 -> 16:00"
                };
            }

            var bookingToday = await _dbContext.Bookings.Where(x => x.BookingDate == booking.BookingDate).ToListAsync();
            if(booking.IsOverlapWith(bookingToday))
            {
                _logger.LogWarning("Overlap with other booking.");
                return new BussinessResultDto
                {
                    ErrorCode = BusinessErrorCode.Conflict,
                    Message = "Overlap with other booking."
                };
            }

            await _dbContext.Bookings.AddAsync(booking, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BussinessResultDto
            {
                Data = new BookingDto { BookingId = booking.BookingId },
                ErrorCode = BusinessErrorCode.Success,
            };
        }
    }
}