
namespace Utility.Descriptors;

internal record BooleanValue(Descriptor Descriptor, object Instance) : ValueDescriptor<bool>(Descriptor, Instance)
{
}

internal record NullableBooleanValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<bool>(Descriptor, Instance)
{
}

internal record ByteValue(Descriptor Descriptor, object Instance) : ValueDescriptor<byte>(Descriptor, Instance)
{
}

internal record NullableByteValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<byte>(Descriptor, Instance)
{
}

internal record DateTimeValue(Descriptor Descriptor, object Instance) : ValueDescriptor<DateTime>(Descriptor, Instance)
{
}

internal record TypeValue(Descriptor Descriptor, object Instance) : ValueDescriptor<Type>(Descriptor, Instance)
{
}

internal record NullableDateTimeValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<DateTime>(Descriptor, Instance)
{
}

internal record DoubleValue(Descriptor Descriptor, object Instance) : ValueDescriptor<double>(Descriptor, Instance)
{
}

internal record NullableDoubleValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<double>(Descriptor, Instance)
{
}

internal record EnumValue(Descriptor Descriptor, object Instance) : ValueDescriptor<Enum>(Descriptor, Instance)
{
}

internal record NullableEnumValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<Enum>(Descriptor, Instance)
{
}

internal record GuidValue(Descriptor Descriptor, object Instance) : ValueDescriptor<Guid>(Descriptor, Instance)
{
}

internal record NullableGuidValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<Guid>(Descriptor, Instance)
{
}

internal record IntegerValue(Descriptor Descriptor, object Instance) : ValueDescriptor<int>(Descriptor, Instance)
{
}

internal record NullableIntegerValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<int>(Descriptor, Instance)
{
}

internal record LongValue(Descriptor Descriptor, object Instance) : ValueDescriptor<long>(Descriptor, Instance);


internal record NullableLongValue(Descriptor Descriptor, object Instance) : NullableValueDescriptor<long>(Descriptor, Instance);

internal record NullValue(Descriptor Descriptor, object Instance) : ValuePropertyDescriptor(Descriptor, Instance), IValue
{
    public override IObservable<object> Children => Observable.Empty<object>();

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


internal record StructValue(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance)
{
}


internal record StringValue(Descriptor Descriptor, object Instance) : ValueDescriptor<string>(Descriptor, Instance)
{
}
