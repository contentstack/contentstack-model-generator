using System;
using System.Collections.Generic;
using contentstack.model.generator.Model;

namespace Contentstack.Model.Generator.Model
{
    public class ContentstackResponse
    {
        public List<Contenttype> listContentTypes { get; set; }

        public int Count { get; set; }

        public ContentstackResponse()
        {
            
        }
    }
}
