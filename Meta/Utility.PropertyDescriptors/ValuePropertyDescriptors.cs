
namespace Utility.Descriptors;

public record BooleanValue(Descriptor Descriptor, object Instance) : ValueDescriptor<bool>(Descriptor, Instance)
{
}

public record NullableBooleanValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<bool>(Descriptor, Instance)
{
}

public record ByteValue(Descriptor Descriptor, object Instance) : ValueDescriptor<byte>(Descriptor, Instance)
{
}

public record NullableByteValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<byte>(Descriptor, Instance)
{
}

public record DateTimeValue(Descriptor Descriptor, object Instance) : ValueDescriptor<DateTime>(Descriptor, Instance)
{
}

public record TypeValue(Descriptor Descriptor, object Instance) : ValueDescriptor<System.Type>(Descriptor, Instance)
{
}

public record NullableDateTimeValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<DateTime>(Descriptor, Instance)
{
}

public record DoubleValue(Descriptor Descriptor, object Instance) : ValueDescriptor<double>(Descriptor, Instance)
{
}

public record NullableDoubleValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<double>(Descriptor, Instance)
{
}

public record EnumValue(Descriptor Descriptor, object Instance) : ValueDescriptor<Enum>(Descriptor, Instance)
{
}

public record NullableEnumValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<Enum>(Descriptor, Instance)
{
}

public record GuidValue(Descriptor Descriptor, object Instance) : ValueDescriptor<Guid>(Descriptor, Instance)
{
}

public record NullableGuidValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<Guid>(Descriptor, Instance)
{
}

public record IntegerValue(Descriptor Descriptor, object Instance) : ValueDescriptor<int>(Descriptor, Instance)
{
}

public record NullableIntegerValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<int>(Descriptor, Instance)
{
}

public record LongValue(Descriptor Descriptor, object Instance) : ValueDescriptor<long>(Descriptor, Instance);


public record NullableLongValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<long>(Descriptor, Instance);

public record NullValue(Descriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IValue
{
    public override object? Get()
    {
        return null;
    }
}


public record ObjectValue(Descriptor Descriptor, object Instance) : ReferenceDescriptor(Descriptor, Instance);

public record StructValue(Descriptor Descriptor, object Instance) : ValueDescriptor<object>(Descriptor, Instance)
{
}

public record StringValue(Descriptor Descriptor, object Instance) : ValueDescriptor<string>(Descriptor, Instance)
{
}
