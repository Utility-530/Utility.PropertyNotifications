using System;
using System.Collections;

namespace Utility.Interfaces.NonGeneric;

public interface IProperty
{
    string Name { get; }
    Type PropertyType { get; }
    bool IsValueType { get; }
    bool IsReadOnly { get; }
    bool IsFlagsEnum { get; }
    //bool IsEnum => PropertyType.IsEnum;
    bool IsEnum { get; }
    object Value { get; set; }
    //public bool IsCollection => PropertyType != null && PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType);
    public bool IsCollection { get; } //=> PropertyType != null && PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType);
}

