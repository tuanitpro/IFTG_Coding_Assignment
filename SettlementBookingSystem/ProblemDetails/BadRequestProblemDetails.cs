using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace SettlementBookingSystem.ProblemDetails
{
    public class BadRequestProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public BadRequestProblemDetails(ValidationException ex)
        {
            Status = StatusCodes.Status400BadRequest;
            Title = "Bad Request";
            Detail = string.Join(";", ex.Errors);
            Type = "https://httpstatuses.com/400";
        }
    }
}
