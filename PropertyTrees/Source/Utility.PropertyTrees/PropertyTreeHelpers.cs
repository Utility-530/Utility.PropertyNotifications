
using System.Linq;
using Utility.Trees;

namespace Utility.PropertyTrees
{
    public static class PropertyTreeHelpers
    {
        public static bool IsDescendantOfCollection(this PropertyBase propertyBase)
        {
            if (propertyBase.IsChildOfCollection())
            {
                return true;
            }
            else if (propertyBase.Parent is PropertyBase parent)
            {
                return parent.IsDescendantOfCollection();
            }
            return false;
        }

        public static bool IsChildOfCollection(this PropertyBase propertyBase)
        {
            if (propertyBase.Parent is ReferenceProperty { IsCollection: true })
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<PropertyBase> SelfAndAncestors(this PropertyBase propertyBase)
        {
            return new[] { propertyBase }.Concat(propertyBase.Ancestors().Cast<PropertyBase>());
        }
    }
}
