using System.Collections.Generic;
using Newtonsoft.Json;

namespace contentstack.model.generator.Model
{
    public class Field
    {
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        public string Uid { get; set; }

        [JsonProperty(PropertyName = "data_type")]
        public string DataType { get; set; }

        [JsonProperty(PropertyName = "multiple")]
        public bool IsMultiple { get; set; }

        [JsonProperty(PropertyName = "reference_to")]
        public object ReferenceTo { get; set; }

        [JsonProperty(PropertyName = "field_metadata")]
        public MetaData Fieldmetadata { get; set; }

        public List<Contenttype> Blocks { get; set; }

        public List<Field> Schema { get; set; }
    }
}
