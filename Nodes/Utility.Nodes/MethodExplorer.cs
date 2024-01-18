using System.ComponentModel;
using Utility.Observables.Generic;
using System.Reflection;
using Fasterflect;

namespace Utility.Nodes
{
    public static class MethodExplorer
    {
        public static IEnumerable<MethodInfo> MethodInfos(PropertyDescriptor propertyDescriptor)
        {
            return
               propertyDescriptor
               .PropertyType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => true)
                .Cast<MethodInfo>()
                .OrderBy(d => d.Name);
        }

        public static IEnumerable<ParameterDescriptor> ParameterDescriptors(this MethodInfo methodInfo)
        {
            return
               methodInfo
               .Parameters()
                .Select(a => new ParameterDescriptor(a));
        }
    }
}

