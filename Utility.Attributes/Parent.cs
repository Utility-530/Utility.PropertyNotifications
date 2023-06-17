using System;

namespace UtilityAttribute
{
    public class ParentAttribute : Attribute
    {
        public readonly string Name;

        public ParentAttribute(string name="")
        {
            this.Name = name;
        }
    }
}
