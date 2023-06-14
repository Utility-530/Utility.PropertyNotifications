using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IDoubleValue
    {
        double Value { get; }
    }

    public interface IIntValue
    {
        int Value { get; }
    }   
    
    public interface IBooleanValue
    {
        bool Value { get; }
    }

    public interface ILongValue
    {
        long Value { get; }
    }

    public interface IFloatValue
    {
        float Value { get; }
    }

    public interface IEnumValue
    {
        Enum Value { get; }
    }

    public interface IShortValue
    {
        short Value { get; }
    }

    public interface IStringValue
    {
        string Value { get; }
    }   
    
    public interface IValue
    {
        object Value { get; }
    }
}
