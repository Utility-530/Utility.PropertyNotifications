using System.Collections;

namespace Utility.PropertyTrees.Abstractions;

public interface IProperty
{
    string Name { get; }
    Type PropertyType { get; }
    bool IsValueType { get; }
    bool IsReadOnly { get; }
    bool IsFlagsEnum { get; }
    bool IsEnum => PropertyType.IsEnum;
    object Value { get; set; }
    public virtual bool IsCollection => PropertyType != null && PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(PropertyType);
}

