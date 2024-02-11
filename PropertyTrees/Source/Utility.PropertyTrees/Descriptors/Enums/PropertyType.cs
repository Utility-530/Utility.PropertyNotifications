using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.PropertyDescriptors
{
    [Flags]
    public enum PropertyType
    {
        Root = 1,
        Reference = 2,
        Value = 4,
        CollectionItem = 8
    }
}
