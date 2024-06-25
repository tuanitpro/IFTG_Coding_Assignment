using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SettlementBookingSystem.Application.Bookings.Dtos;
using SettlementBookingSystem.Application.Services;
using SettlementBookingSystem.Application.SharedDto;

namespace SettlementBookingSystem.Application.Bookings.Commands
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BussinessResultDto>
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger _logger;

        public CreateBookingCommandHandler(IBookingService bookingService, ILogger<CreateBookingCommandHandler> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        public async Task<BussinessResultDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
             _logger.LogInformation("Create new booking Name: {Name} BookingTime: {BookingTime}", request.Name, request.BookingTime);
            // var bookingRequestDto = _mapper.Map<BookingRequestDto>(request);
            var result = await _bookingService.AddAsync(new BookingRequestDto
            {
                BookingTime= request.BookingTime,
                 Name= request.Name,
            }, cancellationToken);
            return result;
        }
    }
}