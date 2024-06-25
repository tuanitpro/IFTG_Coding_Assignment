using Microsoft.AspNetCore.Http;
using SettlementBookingSystem.Application.Exceptions;

namespace SettlementBookingSystem.ProblemDetails
{
    public class ConflictProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public ConflictProblemDetails(ConflictException ex)
        {
            Status = StatusCodes.Status409Conflict;
            Title = "Conflict";
            Detail = ex?.Message;
            Type = "https://httpstatuses.com/409";
        }
    }
}
