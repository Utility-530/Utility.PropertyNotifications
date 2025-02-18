using System.Collections.Generic;

namespace Utility.Nodes.Demo.MVVM
{
    public class RazorModel
    {
        public Controls Controls { get; set; }
        public List<Product> Product { get; set; } = new();
        public Product SelectedProduct { get; set; }
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
