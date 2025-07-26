using System.Reflection;
using Utility.Interfaces.Exs;

namespace Utility.Models
{
    public class Method(MethodInfo methodInfo, object? instance) : IMethod
    {
        Lazy<ParameterInfo[]> names = new(() => [.. methodInfo.GetParameters()]);

        public MethodInfo MethodInfo { get; } = methodInfo;
        public object? Instance { get; } = instance;
        public string Name => MethodInfo.Name;

        public IReadOnlyCollection<ParameterInfo> Parameters => names.Value;

        public override bool Equals(object? obj)
        {
            if (obj is Method { MethodInfo: { } info } method)
            {
                if (this.MethodInfo.Equals(info))
                    return true;
            }
            return base.Equals(obj);
        }


        public override int GetHashCode()
        {
            return MethodInfo.GetHashCode();
        }

    }
}
