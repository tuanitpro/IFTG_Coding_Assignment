using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.Services;
using SettlementBookingSystem.Application.SharedDto;
using SettlementBookingSystem.Controllers;
using Xunit;

namespace SettlementBookingSystem.Application.UnitTests.Controllers
{
    public class BasicBookingControllerTests
    {
        private readonly BasicBookingController _controller;
        private readonly Mock<IBookingService> _bookingService;

        private readonly ILogger<BasicBookingController> _logger;

        public BasicBookingControllerTests()
        {
            _bookingService = new Mock<IBookingService>();

            var mockLogger = new Mock<ILogger<BasicBookingController>>();

            _logger = mockLogger.Object;

            var context = new DefaultHttpContext
            {
                Connection =
                {
                    Id = Guid.NewGuid().ToString()
                }
            };

            _controller = new BasicBookingController(_bookingService.Object,
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

            _bookingService.Setup(x => x.AddAsync(It.IsAny<BookingRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var actual = await _controller.Create(new BookingRequestDto()
            {
                BookingTime = bookingTime,
                Name = "Guest",
            });
            Assert.IsType<OkObjectResult>(actual);
            _bookingService.Verify();
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

            _bookingService.Setup(x => x.AddAsync(It.IsAny<BookingRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var actual = await _controller.Create(new BookingRequestDto()
            {
                BookingTime = "invalid",
                Name = "Guest",
            });
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

            _bookingService.Setup(x => x.AddAsync(It.IsAny<BookingRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var actual = await _controller.Create(new BookingRequestDto()
            {
                BookingTime = bookingTime,
                Name = "Guest",
            });
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

            _bookingService.Setup(x => x.AddAsync(It.IsAny<BookingRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(bookingResultMock);
            var actual = await _controller.Create(new BookingRequestDto()
            {
                BookingTime = bookingTime,
                Name = "Guest",
            });
            Assert.IsType<ConflictObjectResult>(actual);
            var badRequestResult = Assert.IsType<ConflictObjectResult>(actual).Value;
            Assert.IsType<BussinessResultDto>(badRequestResult);
            Assert.Equal("Overlap with other booking.", (badRequestResult as BussinessResultDto).Message);
        }
    }
}