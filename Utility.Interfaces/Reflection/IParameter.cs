using System.Reflection;

namespace Utility.Interfaces.Methods
{
    public interface IParameter:IParameterInfo
    {
        IMethod Method { get; }
        public string Name { get; }
    }
}