using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SettlementBookingSystem.Application.Bookings.Commands;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.Exceptions;
using SettlementBookingSystem.Application.Services;
using SettlementBookingSystem.Application.SharedDto;
using SettlementBookingSystem.Controllers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SettlementBookingSystem.Application.UnitTests
{
    public class CreateBookingCommandHandlerTests
    {
        private readonly CreateBookingCommandHandler _handler;
        private readonly Mock<IBookingService> _bookingService;
        private readonly ILogger<CreateBookingCommandHandler> _logger;

        public CreateBookingCommandHandlerTests()
        {
            _bookingService = new Mock<IBookingService>();

            var mockLogger = new Mock<ILogger<CreateBookingCommandHandler>>();

            _logger = mockLogger.Object;

            _handler = new CreateBookingCommandHandler(_bookingService.Object, _logger);
        }

        [Fact]
        public async Task GivenValidBookingTime_WhenNoConflictingBookings_ThenBookingIsAccepted()
        {
            var command = new CreateBookingCommand
            {
                Name = "test",
                BookingTime = "09:15",
            };
            var bookingDto = new BookingDto
            {
                BookingId = Guid.NewGuid(),
            };

            var bookingResultMock = new BussinessResultDto
            {
                Data = bookingDto,
                ErrorCode = BusinessErrorCode.Success,
            };

            _bookingService.Setup(x => x.AddAsync(It.IsAny<BookingRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GivenOutOfHoursBookingTime_WhenBooking_ThenValidationFails()
        {
            var command = new CreateBookingCommand
            {
                Name = "test",
                BookingTime = "00:00",
            };

            var bookingResultMock = new BussinessResultDto
            {
                ErrorCode = BusinessErrorCode.InvalidData,
                Message = "Out of hours. Time allowed: 09:00 -> 16:00"
            };

            _bookingService.Setup(x => x.AddAsync(It.IsAny<BookingRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var result = await _handler.Handle(command, CancellationToken.None);
            
            Assert.Equal(BusinessErrorCode.InvalidData, result.ErrorCode);
            Assert.Equal("Out of hours. Time allowed: 09:00 -> 16:00", result.Message);

        }

        [Fact]
        public async Task GivenValidBookingTime_WhenBookingIsFull_ThenConflictThrown()
        {
            var command = new CreateBookingCommand
            {
                Name = "test",
                BookingTime = "09:15",
            };

            var bookingResultMock = new BussinessResultDto
            {
                ErrorCode = BusinessErrorCode.Conflict,
                Message = "Overlap with other booking."
            };

            _bookingService.Setup(x => x.AddAsync(It.IsAny<BookingRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.Equal(BusinessErrorCode.Conflict, result.ErrorCode);
            Assert.Equal("Overlap with other booking.",result.Message);
        }
    }
}