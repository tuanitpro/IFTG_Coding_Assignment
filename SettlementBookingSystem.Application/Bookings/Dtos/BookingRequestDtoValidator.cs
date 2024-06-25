using FluentValidation;

namespace SettlementBookingSystem.Application.Bookings.Dtos
{
    public class BookingRequestDtoValidator : AbstractValidator<BookingRequestDto>
    {
        public BookingRequestDtoValidator()
        {
            RuleFor(b => b.Name).NotEmpty();
            RuleFor(b => b.BookingTime).Matches(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9](?:-(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9])?$");
        }
    }
}
