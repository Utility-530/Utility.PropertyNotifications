using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees.WPF.Demo.Infrastructure
{
    class TypeFilter : IPredicate
    {
        readonly string[] names = new[] { "Name", "FullName" };
        public bool Invoke(object value)
        {
            if (value is IPropertyInfo propertyInfo)
            {
                var contains = names.Contains(propertyInfo.Property.Name);
                return contains;
            }
            return false;
        }
    }
}
