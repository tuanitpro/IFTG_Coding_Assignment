using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SettlementBookingSystem.Application.Entities;

namespace SettlementBookingSystem.Application.Helpers
{
    public static class BookingHelperExtension
    {
        public static bool IsBetweenTimeRange(this Booking booking, TimeSpan start, TimeSpan end)
        {
            if (booking.StartTime <= booking.EndTime)
                return booking.StartTime >= start && booking.StartTime <= end && booking.EndTime <= end;

            return false;
        }

        public static Booking ToBookingWithTimeRange(this Booking booking, string bookingTime)
        {
            var timeRanges = bookingTime?.Split('-');
            if (timeRanges?.Length > 1)
            {
                booking.StartTime = TimeSpan.ParseExact(timeRanges[0], @"hh\:mm", CultureInfo.InvariantCulture);
                booking.EndTime = TimeSpan.ParseExact(timeRanges[1], @"hh\:mm", CultureInfo.InvariantCulture);
            }
            else
            {
                booking.StartTime = TimeSpan.ParseExact(timeRanges[0], @"hh\:mm", CultureInfo.InvariantCulture);
                booking.EndTime = booking.StartTime.Add(new TimeSpan(0, 59, 59));
            }
            booking.BookingDate = DateOnly.FromDateTime(DateTime.Now);
            return booking;
        }

        public static bool IsBookingOverlapping(this Booking booking, IEnumerable<Booking> otherBookings)
        {
            if (otherBookings?.Count() == 0) return false;
            var isOverlapping = otherBookings.Any(x => x.StartTime == booking.StartTime);
            if (!isOverlapping)
            {
                isOverlapping = otherBookings.Any(x => (x.StartTime <= booking.StartTime && x.StartTime >= booking.EndTime));
            }
            return isOverlapping;
        }

        public static bool IsOverlapWith(this Booking booking, IEnumerable<Booking> otherBookings)
        {
            if (otherBookings?.Count() < 4) return false;
            var resultOverlap = new List<Booking>();
            foreach (var item in otherBookings)
            {
                var isOverlapping = item.StartTime == booking.StartTime;
                if (!isOverlapping)
                {
                    isOverlapping = item.StartTime <= booking.StartTime && item.StartTime >= booking.EndTime;
                }
                if(isOverlapping)
                {
                    resultOverlap.Add(item);
                }
            };
            return resultOverlap?.Count >= 4;
        }
    }
}