using System.Collections.Generic;
using Newtonsoft.Json;

namespace contentstack.model.generator.Model
{
    public class Contenttype
    {
        public string Title { get; set; }

        public string Uid { get; set; }

        public List<Field> Schema { get; set; }

        [JsonProperty(propertyName: "reference_to")]
        public string ReferenceTo { get; set; }
    }
}
