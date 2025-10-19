using System.Reflection;

namespace Utility.Interfaces.Methods
{
    public interface IParameter
    {
        IMethod Method { get; }
        ParameterInfo? Info { get; }
        public string Name { get; }
    }
}
