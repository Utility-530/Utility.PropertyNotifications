using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.WPF.Meta
{
    public class ViewAttribute : Attribute
    {
        public ViewAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; }
    }
}
