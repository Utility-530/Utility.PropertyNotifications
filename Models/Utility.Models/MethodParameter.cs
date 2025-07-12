using Splat;
using System.Reflection;

namespace Utility.Models
{
    public interface IMethodParameter
    {
        Method Method { get; }
        ParameterInfo Parameter { get; }
        public string Name { get; }
    }

    public record MethodParameter<T>(string InfoName, string Name = "", object? Instance = null) : IMethodParameter
    {
        private readonly Lazy<Method> info = new(() =>
        {
            if (typeof(T).GetMethod(InfoName) is MethodInfo { IsStatic: var isStatic } methodInfo)
            {
                if (isStatic == false && Instance == null)
                {
                    Instance = Locator.Current.GetService(typeof(T)) ?? throw new Exception("£36566");
                }
                return new Method(methodInfo, Instance);
            }
            throw new Exception("ee3433333");
        });
        public Method Method => info.Value;
        public ParameterInfo Parameter => info.Value.MethodInfo.GetParameters().Single(a => a.Name == Name);
    }
}
