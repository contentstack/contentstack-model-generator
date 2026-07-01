using System.Text.Json.Serialization;

namespace contentstack.model.generator.Model
{
    public class MetaData
    {
        [JsonPropertyName("ref_multiple")]
        public bool RefMultiple { get; set; }

        [JsonPropertyName("default_value")]
        public object DefaultValue { get; set; }

        [JsonPropertyName("ref_multiple_content_types")]
        public bool RefMultipleContentType { get; set; }

        [JsonPropertyName("allow_rich_text")]
        public bool IsRichText { get; set; }

        [JsonPropertyName("markdown")]
        public bool IsMarkdown { get; set; }

        [JsonPropertyName("extension")]
        public bool IsExtension { get; set; }

        [JsonPropertyName("allow_json_rte")]
        public bool IsJsonRTE { get; set; }
    }
}
