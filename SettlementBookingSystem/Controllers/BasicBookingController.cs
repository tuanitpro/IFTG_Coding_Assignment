using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.Services;
using SettlementBookingSystem.Application.SharedDto;

namespace SettlementBookingSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasicBookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger _logger;

        public BasicBookingController(IBookingService bookingService, ILogger<BasicBookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingRequestDto bookingDto)
        {
            _logger.LogInformation("Create new booking Name: {Name} BookingTime: {BookingTime}", bookingDto.Name, bookingDto.BookingTime);

            var validator = new BookingRequestDtoValidator();
            var results = validator.Validate(bookingDto);
            if (!results.IsValid)
            {
                return BadRequest(new BussinessResultDto
                {
                    ErrorCode = BusinessErrorCode.InvalidData,
                    Message = string.Join("", results.Errors.Select(x => x.ErrorMessage).ToArray())
                });
            }

            var result = await _bookingService.AddAsync(bookingDto);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            _logger.LogWarning($"Cannot create booking because ${result.Message}");
            if(result.ErrorCode== BusinessErrorCode.Conflict)
            {
                return Conflict(result);
            }
            return BadRequest(result);
        }
    }
}