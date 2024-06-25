using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.Contexts;
using SettlementBookingSystem.Application.Entities;
using SettlementBookingSystem.Application.Helpers;
using SettlementBookingSystem.Application.Services;
using SettlementBookingSystem.Application.SharedDto;
using Xunit;

namespace SettlementBookingSystem.Application.UnitTests.Services
{
    public class BookingServiceTests
    {
        private readonly BookingContext _dbContext;
        private readonly IBookingService _bookingService;

        private readonly ILogger<BookingService> _logger;
        private readonly IMapper _mapper;

        public BookingServiceTests()
        {
            var mockLogger = new Mock<ILogger<BookingService>>();

            _logger = mockLogger.Object;
            var context = new DefaultHttpContext
            {
                Connection =
                {
                    Id = Guid.NewGuid().ToString()
                }
            };
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BookingRequestDto, Booking>();
            });

            _mapper = config.CreateMapper();
            _dbContext = new InMemoryDbContextFactory().GetDbContext();
            _bookingService = new BookingService(_dbContext, _mapper, _logger);
        }

        [Theory]
        [InlineData("09:00-12:20")]
        [InlineData("09:00")]
        public async Task CreateBooking_WhenCalled_ReturnOk(string bookingTime)
        {
            var booking = new Entities.Booking
            {
            };
            booking = booking.ToBookingWithTimeRange(bookingTime);
            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();
            var actual = await _bookingService.AddAsync(new BookingRequestDto()
            {
                BookingTime = bookingTime,
                Name = "Guest",
            });
            Assert.IsType<BussinessResultDto>(actual);
            Assert.True(actual.IsSuccess);
        }

        [Theory]
        [InlineData("08:00-11:20")]
        [InlineData("08:30")]
        [InlineData("18:00-11:20")]
        [InlineData("09:00-17:20")]
        public async Task CreateBooking_OutOfHours_BookingTime_ReturnBadRequuest(string bookingTime)
        {
            var actual = await _bookingService.AddAsync(new BookingRequestDto()
            {
                BookingTime = bookingTime,
                Name = "Guest",
            });
            Assert.IsType<BussinessResultDto>(actual);
            Assert.False(actual.IsSuccess);
            Assert.Equal(BusinessErrorCode.InvalidData, actual.ErrorCode);
            Assert.Equal("Out of hours. Time allowed: 09:00 -> 16:00", actual.Message);
        }

        [Theory]
        [InlineData("09:00-09:30")]
        public async Task CreateBooking_OverlapWith_BookingTime_ReturnConflict(string bookingTime)
        {
            var bookings = new List<Booking> {
                new Entities.Booking
                {
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    BookingTime = bookingTime,
                    Name = "Name 1"
                },
                new Entities.Booking
                {
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    BookingTime = bookingTime,
                    Name = "Name 2"
                },
                new Entities.Booking
                {
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    BookingTime = bookingTime,
                    Name = "Name 3"
                },
                new Entities.Booking
                {
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                    BookingTime = bookingTime,
                    Name = "Name 4"
                }
            };
            await _dbContext.Bookings.AddRangeAsync(bookings);
            await _dbContext.SaveChangesAsync();

            var actual = await _bookingService.AddAsync(new BookingRequestDto()
            {
                BookingTime = bookingTime,
                Name = "Guest",
            });
            Assert.IsType<BussinessResultDto>(actual);
            Assert.False(actual.IsSuccess);
            Assert.Equal(BusinessErrorCode.Conflict, actual.ErrorCode);
            Assert.Equal("Overlap with other booking.", actual.Message);
        }
    }
}