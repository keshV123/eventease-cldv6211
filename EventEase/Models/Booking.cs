namespace EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public string? CustomerName { get; set; }
        public string? Email { get; set; }

        public int Tickets { get; set; }

        public int EventId { get; set; }

        public Event? Event { get; set; } 
    }
}