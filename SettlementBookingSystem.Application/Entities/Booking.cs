using System;
using System.ComponentModel.DataAnnotations;

namespace SettlementBookingSystem.Application.Entities
{
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; }

        public string Name { get; set; }

        public DateOnly BookingDate { get; set; }

        public TimeSpan StartTime {  get; set; }

        public TimeSpan EndTime { get; set; }

        public string BookingTime { get; set; }
    }
}