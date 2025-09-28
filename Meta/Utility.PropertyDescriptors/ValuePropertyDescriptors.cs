
namespace Utility.PropertyDescriptors;

internal class BooleanValue(Descriptor Descriptor, object Instance) : ValueDescriptor<bool>(Descriptor, Instance)
{
}

internal class NullableBooleanValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<bool>(Descriptor, Instance)
{
}

internal class ByteValue(Descriptor Descriptor, object Instance) : ValueDescriptor<byte>(Descriptor, Instance)
{
}

internal class NullableByteValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<byte>(Descriptor, Instance)
{
}

internal class DateTimeValue(Descriptor Descriptor, object Instance) : ValueDescriptor<DateTime>(Descriptor, Instance)
{
}

internal class TypeValue(Descriptor Descriptor, object Instance) : ValueDescriptor<Type>(Descriptor, Instance)
{
}

internal class NullableDateTimeValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<DateTime>(Descriptor, Instance)
{
}

internal class DoubleValue(Descriptor Descriptor, object Instance) : ValueDescriptor<double>(Descriptor, Instance)
{
}

internal class NullableDoubleValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<double>(Descriptor, Instance)
{
}

internal class EnumValue(Descriptor Descriptor, object Instance) : ValueDescriptor<Enum>(Descriptor, Instance)
{
}

internal class NullableEnumValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<Enum>(Descriptor, Instance)
{
}

internal class GuidValue(Descriptor Descriptor, object Instance) : ValueDescriptor<Guid>(Descriptor, Instance)
{
}

internal class NullableGuidValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<Guid>(Descriptor, Instance)
{
}

internal class IntegerValue(Descriptor Descriptor, object Instance) : ValueDescriptor<int>(Descriptor, Instance)
{
}

internal class NullableIntegerValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<int>(Descriptor, Instance)
{
}

internal class LongValue(Descriptor Descriptor, object Instance) : ValueDescriptor<long>(Descriptor, Instance);


internal class NullableLongValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<long>(Descriptor, Instance);

internal class NullValue(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance), IValue
{
    public override IEnumerable<object> Children => Array.Empty<object>();

    public override bool HasChildren => false;

    public override object? Get()
    {
        return null;
    }


    public override void Initialise(object? item = null)
    {
    }

    public override void Finalise(object? item = null)
    {
    }

    public override void Set(object? value)
    {
        throw new NotImplementedException();
    }
}


internal class StructValue(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance)
{
}


internal class StringValue(Descriptor Descriptor, object Instance) : ValueDescriptor<string>(Descriptor, Instance)
{
}
