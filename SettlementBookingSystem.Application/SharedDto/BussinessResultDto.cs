namespace SettlementBookingSystem.Application.SharedDto
{

    public class BussinessResultDto
    {
        

        public dynamic Data { get; set; }

        public string Message { get; set; }

        public BusinessErrorCode ErrorCode { get; set; }

        public bool IsSuccess => ErrorCode == BusinessErrorCode.Success;
    }
}
