using System;
using System.Collections.Generic;

namespace EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string? ImageUrl { get; set; }

        public string Description { get; set; }

        // Foreign Key
        public int VenueId { get; set; }

        public Venue? Venue { get; set; }

        public List<Booking>? Bookings { get; set; }
    }
}