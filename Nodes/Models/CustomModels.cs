using System.Collections.Generic;

namespace CustomModels
{
    public class RazorModel
    {
        public Controls Controls { get; set; }
        public List<Product> Products { get; set; } = new();
    }

    public class Controls
    {
        public bool IsTransforming { get; set; }


    }

    public class Product
    {
        public List<Description> Descriptions { get; set; }
    }

    public class Description
    {
        public string Value { get; set; }
    }


}
