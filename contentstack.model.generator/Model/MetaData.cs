using Newtonsoft.Json;

namespace contentstack.model.generator.Model
{
    public class MetaData
    {
        [JsonProperty(PropertyName = "ref_multiple")]
        public bool RefMultiple { get; set; }

        [JsonProperty(PropertyName = "default_value")]
        public object DefaultValue { get; set; }

        [JsonProperty(PropertyName = "ref_multiple_content_types")]
        public bool RefMultipleContentType { get; set; }

        [JsonProperty(PropertyName = "allow_rich_text")]
        public bool IsRichText { get; set; }

        [JsonProperty(PropertyName = "markdown")]
        public bool IsMarkdown { get; set; }

        [JsonProperty(PropertyName = "extension")]
        public bool IsExtension { get; set; }

        [JsonProperty(PropertyName = "allow_json_rte")]
        public bool IsJsonRTE { get; set; }
    }
}
