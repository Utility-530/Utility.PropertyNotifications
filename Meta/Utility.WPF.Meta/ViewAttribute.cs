using System;

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