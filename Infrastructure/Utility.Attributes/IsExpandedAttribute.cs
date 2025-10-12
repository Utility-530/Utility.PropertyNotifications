using System;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IsExpandedAttribute : Attribute
    {
        public IsExpandedAttribute(bool isExpanded)
        {
            IsExpanded = isExpanded;
        }

        public bool IsExpanded { get; }
    }
}