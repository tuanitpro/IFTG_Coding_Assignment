using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SettlementBookingSystem.Application.Bookings.Commands;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.SharedDto;
using SettlementBookingSystem.Controllers;
using Xunit;

namespace SettlementBookingSystem.Application.UnitTests.Controllers
{
    public class MediatorBookingControllerTests
    {
        private readonly MediatorBookingController _controller;
        private readonly Mock<IMediator> _mediator;

        private readonly ILogger<MediatorBookingController> _logger;

        public MediatorBookingControllerTests()
        {
            _mediator = new Mock<IMediator>();

            var mockLogger = new Mock<ILogger<MediatorBookingController>>();

            _logger = mockLogger.Object;

            var context = new DefaultHttpContext
            {
                Connection =
                {
                    Id = Guid.NewGuid().ToString()
                }
            };

            _controller = new MediatorBookingController(_mediator.Object,
                _logger)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = context
                }
            };
        }

        [Theory]
        [InlineData("09:00-17:20")]
        [InlineData("09:00")]
        public async Task CreateBooking_WhenCalled_ReturnOk(string bookingTime)
        {
            var bookingDto = new BookingDto
            {
                BookingId = Guid.NewGuid(),
            };

            var bookingResultMock = new BussinessResultDto
            {
                Data = bookingDto,
                ErrorCode = BusinessErrorCode.Success,
            };
            
            _mediator.Setup(x => x.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var command = new CreateBookingCommand
            {
                Name = "test",
                BookingTime = bookingTime,
            };
            var actual = await _controller.Create(command);
            Assert.IsType<OkObjectResult>(actual);
            _mediator.Verify();
        }

        [Fact]
        public async Task CreateBooking_Invalid_BookingTime_ReturnBadRequuest()
        {
            var bookingDto = new BookingDto
            {
                BookingId = Guid.NewGuid(),
            };

            var bookingResultMock = new BussinessResultDto
            {
                ErrorCode = BusinessErrorCode.InvalidData,
            };

            _mediator.Setup(x => x.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var command = new CreateBookingCommand
            {
                Name = "test",
                BookingTime = "bookingTime",
            };
            var actual = await _controller.Create(command);
            Assert.IsType<BadRequestObjectResult>(actual);
        }

        [Theory]
        [InlineData("08:00-11:20")]
        [InlineData("08:30")]
        public async Task CreateBooking_OutOfHours_BookingTime_ReturnBadRequuest(string bookingTime)
        {
            var bookingResultMock = new BussinessResultDto
            {
                ErrorCode = BusinessErrorCode.InvalidData,
                Message = "Out of hours. Time allowed: 09:00 -> 16:00"
            };

            _mediator.Setup(x => x.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var command = new CreateBookingCommand
            {
                Name = "test",
                BookingTime = bookingTime,
            };
            var actual = await _controller.Create(command);
            Assert.IsType<BadRequestObjectResult>(actual);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual).Value;
            Assert.IsType<BussinessResultDto>(badRequestResult);
            Assert.Equal("Out of hours. Time allowed: 09:00 -> 16:00", (badRequestResult as BussinessResultDto).Message);
        }

        [Theory]
        [InlineData("09:00-11:20")]
        public async Task CreateBooking_OverlapWith_BookingTime_ReturnConflict(string bookingTime)
        {
            var bookingResultMock = new BussinessResultDto
            {
                ErrorCode = BusinessErrorCode.Conflict,
                Message = "Overlap with other booking."
            };

            _mediator.Setup(x => x.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var command = new CreateBookingCommand
            {
                Name = "test",
                BookingTime = bookingTime,
            };
            var actual = await _controller.Create(command);
            Assert.IsType<ConflictObjectResult>(actual);
            var badRequestResult = Assert.IsType<ConflictObjectResult>(actual).Value;
            Assert.IsType<BussinessResultDto>(badRequestResult);
            Assert.Equal("Overlap with other booking.", (badRequestResult as BussinessResultDto).Message);
        }
    }
}