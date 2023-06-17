using System;

namespace UtilityAttribute
{
    public class ChildAttribute : Attribute
    {
        public readonly string Name;

        public ChildAttribute(string name="")
        {
            this.Name = name;
        }
    }
}
