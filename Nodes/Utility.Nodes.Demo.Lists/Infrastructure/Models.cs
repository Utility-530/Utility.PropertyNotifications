using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{
    public class ModelAttribute : Attribute
    {

    }
    [ModelAttribute]
    public class Model
    {
        public int Id { get; set; }
    }
   

    [ModelAttribute]
    public class EbayModel
    {
        public int Id { get; set; }
    }
}
