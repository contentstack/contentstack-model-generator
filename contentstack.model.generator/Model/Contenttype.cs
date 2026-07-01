using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace contentstack.model.generator.Model
{
    public class Contenttype
    {
        public string Title { get; set; }

        public string Uid { get; set; }

        public List<Field> Schema { get; set; }

        [JsonPropertyName("reference_to")]
        public string ReferenceTo { get; set; }
    }
}
