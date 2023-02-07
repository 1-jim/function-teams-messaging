using Newtonsoft.Json;

namespace function_teams_messaging.Models
{
    public class TeamsMessageModel
    {
        [JsonProperty("@type")]
        public string Type { get; set; } = "MessageCard";

        [JsonProperty("@context")]
        public string Context { get; set; } = "http://schema.org/extensions";

        public string Summary { get; set; }
        public string ThemeColor { get; set; }
        public Section[] Sections { get; set; }
    }

    public partial class Section
    {
        public string ActivityTitle { get; set; }
        public Fact[] Facts { get; set; }
    }

    public partial class Fact
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}