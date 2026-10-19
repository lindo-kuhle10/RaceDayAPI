using System.Text.Json.Serialization;

namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        public int EventId { get; set; }

        [JsonIgnore]
        public Event? Event { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }

        public string? FinishTime { get; set; }
        public int? Position { get; set; }
    }
}