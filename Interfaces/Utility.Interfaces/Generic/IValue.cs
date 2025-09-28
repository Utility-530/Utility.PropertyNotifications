using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Generic
{
    public interface IValue<T> : IGetValue<T>, ISetValue<T> 
    {
        new T Value { get; set; }
    }
    public interface IGetValue<T> : Utility.Interfaces.NonGeneric.IGetValue
    {
        new T Value { get; }
    }
    public interface ISetValue<T> : Utility.Interfaces.NonGeneric.ISetValue
    {
        new T Value { set; }
    }

}
