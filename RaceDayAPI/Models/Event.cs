namespace RaceDayAPI.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public double Distance { get; set; }
        public string EventType { get; set; } // Run, Walk, Cycle

        public List<Category> Categories { get; set; } = new();
    }
}