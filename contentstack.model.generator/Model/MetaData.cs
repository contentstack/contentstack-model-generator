using System;
using Newtonsoft.Json;

namespace contentstack.model.generator.Model
{
    public class MetaData
    {
        [JsonProperty(PropertyName = "ref_multiple")]
        public bool RefMultiple;

        [JsonProperty(PropertyName = "default_value")]
        public object Defaultvalue;

        [JsonProperty(PropertyName = "ref_multiple_content_types")]
        public bool RefMultipleContentType;
    }
}
