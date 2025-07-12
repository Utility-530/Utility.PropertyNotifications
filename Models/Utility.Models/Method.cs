using System.Reflection;

namespace Utility.Models
{
    public class Method(MethodInfo methodInfo, object? instance)
    {
        Lazy<List<string>> names = new(() => [.. methodInfo.GetParameters().Select(a => a.Name)]);

        public MethodInfo MethodInfo { get; } = methodInfo;
        public object? Instance { get; } = instance;
        public string Name => MethodInfo.Name;

        public object? Execute(params object?[] objects)
        {
            return MethodInfo.Invoke(Instance, objects);
        }

        public object? Execute(Dictionary<string, object> objects)
        {
         
            return MethodInfo.Invoke(Instance, [.. objects.OrderBy(kv => names.Value.IndexOf(kv.Key)).Select(a=>a.Value)]);
        }

        public override bool Equals(object? obj)
        {
            if(obj is Method { MethodInfo: { } info } method)
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
