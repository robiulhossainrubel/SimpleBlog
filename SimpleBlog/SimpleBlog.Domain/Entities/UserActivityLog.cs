using System.Text.Json.Serialization;

namespace SimpleBlog.Domain.Entities
{
    public class UserActivityLog
    {
        [JsonIgnore]
        public ulong Id { get; set; }
        public DateTime EventTime { get; set; }
        public ulong UserId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
