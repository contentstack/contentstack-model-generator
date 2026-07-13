using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace contentstack.model.generator.Model
{
    public class Field
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        public string Uid { get; set; }

        [JsonPropertyName("data_type")]
        public string DataType { get; set; }

        [JsonPropertyName("multiple")]
        public bool IsMultiple { get; set; }

        [JsonPropertyName("reference_to")]
        public object ReferenceTo { get; set; }

        [JsonPropertyName("field_metadata")]
        public MetaData FieldMetadata { get; set; }

        public List<Contenttype> Blocks { get; set; }

        public List<Field> Schema { get; set; }
    }
}
