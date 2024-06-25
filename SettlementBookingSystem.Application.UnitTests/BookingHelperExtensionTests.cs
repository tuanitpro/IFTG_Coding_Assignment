using System;
using System.Collections.Generic;
using SettlementBookingSystem.Application.Entities;
using SettlementBookingSystem.Application.Helpers;
using Xunit;

namespace SettlementBookingSystem.Application.UnitTests
{
    public class BookingHelperExtensionTests
    {
        [Theory]
        [InlineData("09:00-15:20")]
        [InlineData("09:00")]
        public void BookingInTimeRange_Return_True(string bookingTime)
        {
            var booking = new Entities.Booking
            {
            };
            booking = booking.ToBookingWithTimeRange(bookingTime);
            var start = new TimeSpan(09, 00, 00);
            var end = new TimeSpan(16, 00, 00);
            var actual = BookingHelperExtension.IsBetweenTimeRange(booking, start, end);

            Assert.True(actual);
        }

        [Theory]
        [InlineData("09:00-18:20")]
        [InlineData("08:00")]
        public void BookingOutOfHoursTimeRange_Return_False(string bookingTime)
        {
            var booking = new Entities.Booking
            {
            };
            booking = booking.ToBookingWithTimeRange(bookingTime);
            var start = new TimeSpan(09, 00, 00);
            var end = new TimeSpan(16, 00, 00);
            var actual = BookingHelperExtension.IsBetweenTimeRange(booking, start, end);

            Assert.False(actual);
        }

        [Theory]
        [InlineData("09:00")]
        [InlineData("09:15")]
        [InlineData("10:00-11:00")]
        public void IsOverlapingTimeRange_Return_True(string bookingTime)
        {
            var booking = new Entities.Booking
            {
            };
            booking = booking.ToBookingWithTimeRange(bookingTime);
            var otherBookings = new List<Booking>
            {
                new Booking
                {
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 59, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(09, 15, 00),
                    EndTime = new TimeSpan(09, 59, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(10, 00, 00),
                    EndTime = new TimeSpan(11, 59, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(12, 00, 00),
                    EndTime = new TimeSpan(13, 59, 00),
                }
            };
            var actual = BookingHelperExtension.IsBookingOverlapping(booking, otherBookings);

            Assert.True(actual);
        }

        [Theory]
        [InlineData("09:00")]
        [InlineData("09:00-09:30")]
        public void IsOverlapingTimeRange_Return_False(string bookingTime)
        {
            var booking = new Entities.Booking
            {
            };
            booking = booking.ToBookingWithTimeRange(bookingTime);
            var otherBookings = new List<Booking>
            {
                new Booking
                {
                    StartTime = new TimeSpan(10, 00, 00),
                    EndTime = new TimeSpan(10, 59, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(11, 00, 00),
                    EndTime = new TimeSpan(11, 59, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(12, 00, 00),
                    EndTime = new TimeSpan(12, 30, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(13, 00, 00),
                    EndTime = new TimeSpan(13, 59, 00),
                }
            };
            var actual = BookingHelperExtension.IsBookingOverlapping(booking, otherBookings);

            Assert.False(actual);
        }

        [Theory]
        [InlineData("09:00")]
        public void IsOverlapingWith_Return_True(string bookingTime)
        {
            var booking = new Entities.Booking
            {
            };
            booking = booking.ToBookingWithTimeRange(bookingTime);
            var otherBookings = new List<Booking>
            {
                new Booking
                {
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 15, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 30, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 45, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(09, 59, 00),
                }
            };
            var actual = BookingHelperExtension.IsOverlapWith(booking, otherBookings);

            Assert.True(actual);
        }

        [Theory]
        [InlineData("09:00")]
        [InlineData("09:00-09:30")]
        public void IsOverlapingWith_Return_False(string bookingTime)
        {
            var booking = new Entities.Booking
            {
            };
            booking = booking.ToBookingWithTimeRange(bookingTime);
            var otherBookings = new List<Booking>
            {
                new Booking
                {
                    StartTime = new TimeSpan(10, 00, 00),
                    EndTime = new TimeSpan(10, 59, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(11, 00, 00),
                    EndTime = new TimeSpan(11, 59, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(12, 00, 00),
                    EndTime = new TimeSpan(12, 30, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(13, 00, 00),
                    EndTime = new TimeSpan(13, 59, 00),
                },
                new Booking
                {
                    StartTime = new TimeSpan(14, 00, 00),
                    EndTime = new TimeSpan(14, 59, 00),
                }
            };
            var actual = BookingHelperExtension.IsOverlapWith(booking, otherBookings);

            Assert.False(actual);
        }
    }
}