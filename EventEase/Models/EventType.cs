namespace EventEase.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        public string TypeName { get; set; }

        public List<Event>? Events { get; set; }
    }
}
