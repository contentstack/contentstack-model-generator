using System;
using System.Text.Json.Serialization;

namespace Contentstack.Model.Generator.Model
{
    public class StackResponse
    {
        [JsonPropertyName("api_key")]
        public string APIKey { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("master_locale")]
        public string MasterLocale { get; set; }
        public string Uid { get; set; }
        public string Description { get; set; }
        
        public StackSettings Settings { get; set; }
    }

    public class StackSettings
    {
        public DateTime version { get; set; }
    }
}
