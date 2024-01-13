namespace Utility.Interfaces.Generic
{
    public interface IValue<T> : Utility.Interfaces.NonGeneric.IValue
    {
        new T Value { get; }
    }

}
