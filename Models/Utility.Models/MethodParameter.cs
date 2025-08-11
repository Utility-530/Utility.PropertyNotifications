using Splat;
using System.Reflection;
using Utility.Interfaces.Exs;

namespace Utility.Models
{

    public record MethodParameter<T>(string InfoName, string Name = "", object? Instance = null, Type[]? ParameterTypes = null) : IMethodParameter
    {
        private readonly Lazy<Method> info = new(() =>
        {
            if (typeof(T).GetMethods().Where(a => a.Name == InfoName).ToArray() is MethodInfo[] { Length: >0 } methods)
            {
                if (ParameterTypes == null && methods.Length == 1)
                    return method(methods.Single(), Instance);

                if (ParameterTypes != null && methods.SingleOrDefault(m => m.GetParameters().Select(a => a.ParameterType).SequenceEqual(ParameterTypes)) is MethodInfo { IsStatic: var isStatic } methodInfo)
                    return method(methodInfo, Instance);
            }
            throw new Exception("ee3433333");
        });
        public IMethod Method => info.Value;
        public ParameterInfo Parameter => info.Value.MethodInfo.GetParameters().Single(a => a.Name == Name);


        static Method method(MethodInfo methodInfo, object? Instance = null)
        {
            if (methodInfo.IsStatic == false && Instance == null)
            {
                Instance = Locator.Current.GetService(typeof(T)) ?? throw new Exception($"£36566 {typeof(T).Name}");
            }
            return new Method(methodInfo, Instance);
        }
    }
}
