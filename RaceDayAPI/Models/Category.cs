using System.Text.Json.Serialization;

namespace RaceDayAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int EventId { get; set; }

        [JsonIgnore]
        public Event? Event { get; set; }
    }
}