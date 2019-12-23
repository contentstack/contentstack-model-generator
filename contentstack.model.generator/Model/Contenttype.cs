using System;
using System.Collections.Generic;

namespace contentstack.model.generator.Model
{
    public class Contenttype
    {
       public string Title { get; set; }

       public string Uid { get; set; }

       public List<Field> Schema { get; set; }
    }
}
