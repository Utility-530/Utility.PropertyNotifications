using System.ComponentModel;
using System.Reflection;
using Utility.PropertyDescriptors;

namespace Utility.Nodes
{
    public static class MethodExplorer
    {
        public static IEnumerable<MethodInfo> MethodInfos(Type type)
        {
            return Types(type)
            .SelectMany(t => t
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Where(m => true)
            .Cast<MethodInfo>()
            .OrderBy(d => d.Name));
        }

        private static IEnumerable<Type> Types(Type type)
        {
            if (type != typeof(object))
            {
                yield return type;
                foreach (var t in Types(type.BaseType))
                    yield return t;
            }
            else
                yield break;
        }

        public static IEnumerable<ParameterDescriptor> ParameterDescriptors(this MethodInfo methodInfo)
        {
            return
               methodInfo
               .GetParameters()
                .Select(a => new ParameterDescriptor(a));
        }
    }
}

