using System.Linq;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees.Demo.ViewModels
{
    class TypeFilter : IPredicate
    {
        readonly string[] names = new[] { "Name", "FullName" };
        public bool Invoke(object value)
        {
            if (value is IPropertyInfo propertyInfo)
            {
                var contains = names.Contains(propertyInfo.PropertyInfo.Name);
                return contains;
            }
            return false;
        }
    }
}
