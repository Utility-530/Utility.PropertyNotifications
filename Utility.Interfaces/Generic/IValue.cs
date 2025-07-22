namespace Utility.Interfaces.Generic
{
    public interface IValue<T> : Utility.Interfaces.NonGeneric.IValue
    {
        new T Value { get; }
    }
    public interface ISetValue<T> : Utility.Interfaces.NonGeneric.ISetValue
    {
        new T Value { set; }
    }

}
