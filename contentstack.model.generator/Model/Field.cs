using System;
using Newtonsoft.Json;

namespace contentstack.model.generator.Model
{
    public class Field
    {
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName;

        public string Uid;

        [JsonProperty(PropertyName = "data_type")]
        public string DataType;

        [JsonProperty(PropertyName = "multiple")]
        public bool IsMultiple;

        [JsonProperty(PropertyName = "reference_to")]
        public object ReferenceTo;

        [JsonProperty(PropertyName = "field_metadata")]
        public MetaData Fieldmetadata;

    }
}
