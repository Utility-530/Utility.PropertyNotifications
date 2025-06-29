using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class FactoryAttribute: Attribute
    {
    }
}
