using Splat;
using System.Reflection;
using Utility.Interfaces.Exs;

namespace Utility.Models
{

    public record MethodParameter<T>(string InfoName, string Name = "", object? Instance = null) : IMethodParameter
    {
        private readonly Lazy<Method> info = new(() =>
        {
            if (typeof(T).GetMethod(InfoName) is MethodInfo { IsStatic: var isStatic } methodInfo)
            {
                if (isStatic == false && Instance == null)
                {
                    Instance = Locator.Current.GetService(typeof(T)) ?? throw new Exception($"£36566 {typeof(T).Name}");
                }
                return new Method(methodInfo, Instance);
            }
            throw new Exception("ee3433333");
        });
        public IMethod Method => info.Value;
        public ParameterInfo Parameter => info.Value.MethodInfo.GetParameters().Single(a => a.Name == Name);
    }
}
