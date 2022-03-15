using System;
using Newtonsoft.Json;

namespace contentstack.model.generator.Model
{
    public class MetaData
    {
        [JsonProperty(PropertyName = "ref_multiple")]
        public bool RefMultiple { get; set; }

        [JsonProperty(PropertyName = "default_value")]
        public object Defaultvalue { get; set; }

        [JsonProperty(PropertyName = "ref_multiple_content_types")]
        public bool RefMultipleContentType { get; set; }

        [JsonProperty(PropertyName = "allow_rich_text")]
        public bool isRichText { get; set; }

        [JsonProperty(PropertyName = "markdown")]
        public bool isMarkdown { get; set; }
    }
}
