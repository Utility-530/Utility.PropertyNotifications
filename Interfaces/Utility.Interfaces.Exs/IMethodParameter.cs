using System.Reflection;

namespace Utility.Interfaces.Exs
{
    public interface IMethodParameter
    {
        IMethod Method { get; }
        ParameterInfo Parameter { get; }
        public string Name { get; }
    }
}
