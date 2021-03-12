using System;
using Newtonsoft.Json;

namespace Contentstack.Model.Generator.Model
{
    public class StackResponse
    {
        [JsonProperty(PropertyName = "api_key")]
        public string APIKey { get; set; }
        public string Name { get; set; }
        [JsonProperty(PropertyName = "master_locale")]
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
