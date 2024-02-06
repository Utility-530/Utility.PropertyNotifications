using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Changes;
using Utility.Interfaces.NonGeneric;
using Descriptor = System.ComponentModel.PropertyDescriptor;

namespace Utility.PropertyDescriptors
{
    public record BooleanValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<bool>(Descriptor, Instance)
    {
    }

    public record NullableBooleanValue(Descriptor Descriptor, object Instance) : NullablePropertyDescriptor<bool>(Descriptor, Instance)
    {
    }

    public record ByteValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<byte>(Descriptor, Instance)
    {
    }

    public record NullableByteValue(Descriptor Descriptor, object Instance) : NullablePropertyDescriptor<byte>(Descriptor, Instance)
    {
    }

    public record DateTimeValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<DateTime>(Descriptor, Instance)
    {
    }
    public record NullableDateTimeValue(Descriptor Descriptor, object Instance) : NullablePropertyDescriptor<DateTime>(Descriptor, Instance)
    {
    }


    public record DoubleValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<double>(Descriptor, Instance)
    {
    }

    public record NullableDoubleValue(Descriptor Descriptor, object Instance) : NullablePropertyDescriptor<double>(Descriptor, Instance)
    {
    }

    public record EnumValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<Enum>(Descriptor, Instance)
    {
    }

    public record NullableEnumValue(Descriptor Descriptor, object Instance) : NullablePropertyDescriptor<Enum>(Descriptor, Instance)
    {
    }

    public record GuidValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<Guid>(Descriptor, Instance)
    {
    }

    public record NullableGuidValue(Descriptor Descriptor, object Instance) : NullablePropertyDescriptor<Guid>(Descriptor, Instance)
    {
    }

    public record IntegerValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<int>(Descriptor, Instance)
    {
    }
    public record NullableIntegerValue(Descriptor Descriptor, object Instance) : NullablePropertyDescriptor<int>(Descriptor, Instance)
    {
    }


 
    public record LongValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<long>(Descriptor, Instance)
    {
    }

    public record NullableLongValue(Descriptor Descriptor, object Instance) : NullablePropertyDescriptor<long>(Descriptor, Instance)
    {
    }

    public record NullValue(Descriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IValue
    {
        public object Value => null;

        public override IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            return Observable.Empty<Change<IMemberDescriptor>>();
        }
    }


    public record ObjectValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<object>(Descriptor, Instance)
    {
    }

    public record StringValue(Descriptor Descriptor, object Instance) : PropertyDescriptor<string>(Descriptor, Instance)
    {
    }

}
