using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SettlementBookingSystem.Application.Bookings.Commands;
using SettlementBookingSystem.Application.SharedDto;

namespace SettlementBookingSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MediatorBookingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public MediatorBookingController(IMediator mediator, ILogger<MediatorBookingController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
        {
            _logger.LogInformation("Create new booking Name: {Name} BookingTime: {BookingTime}", command.Name, command.BookingTime);

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            _logger.LogWarning($"Cannot create booking because ${result.Message}");
            if (result.ErrorCode == BusinessErrorCode.Conflict)
            {
                return Conflict(result);
            }
            return BadRequest(result);
        }
    }
}