using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Enums;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Structs.Repos;

namespace Utility.Nodes
{
    public partial class NodeViewModel
    {
        private float? floatValue;
        private int? intValue;
        private double? doubleValue;
        private decimal? decimalValue;
        private bool? boolValue;
        private char? charValue;
        private byte? byteValue;
        private sbyte? sByteValue;
        private short? shortValue;
        private long? longValue;
        private uint? uIntValue;
        private ushort? uShortValue;
        private ulong? uLongValue;
        private string? stringValue;
        private IEnumerable _collection;

        public IEnumerable Collection
        {
            get { return _collection; }
            set => this.RaisePropertyChanged(ref this._collection, value);
        }

        public int? IntValue
        {
            get { RaisePropertyCalled(intValue); return intValue; }
            set => this.RaisePropertyReceived(ref this.intValue, value);
        }

        public float? FloatValue
        {
            get { RaisePropertyCalled(floatValue); return floatValue; }
            set => this.RaisePropertyReceived(ref this.floatValue, value);
        }
        public double? DoubleValue
        {
            get { RaisePropertyCalled(doubleValue); return doubleValue; }
            set => this.RaisePropertyReceived(ref this.doubleValue, value);
        }
        public decimal? DecimalValue
        {
            get { RaisePropertyCalled(decimalValue); return decimalValue; }
            set => this.RaisePropertyReceived(ref this.decimalValue, value);
        }
        public bool? BoolValue
        {
            get { RaisePropertyCalled(boolValue); return boolValue; }
            set => this.RaisePropertyReceived(ref this.boolValue, value);
        }
        public char? CharValue
        {
            get { RaisePropertyCalled(charValue); return charValue; }
            set => this.RaisePropertyReceived(ref this.charValue, value);
        }
        public byte? ByteValue
        {
            get { RaisePropertyCalled(byteValue); return byteValue; }
            set => this.RaisePropertyReceived(ref this.byteValue, value);
        }
        public sbyte? SByteValue
        {
            get { RaisePropertyCalled(sByteValue); return sByteValue; }
            set => this.RaisePropertyReceived(ref this.sByteValue, value);
        }
        public short? ShortValue
        {
            get { RaisePropertyCalled(shortValue); return shortValue; }
            set => this.RaisePropertyReceived(ref this.shortValue, value);
        }
        public long? LongValue
        {
            get { RaisePropertyCalled(longValue); return longValue; }
            set => this.RaisePropertyReceived(ref this.longValue, value);
        }
        public uint? UIntValue
        {
            get { RaisePropertyCalled(uIntValue); return uIntValue; }
            set => this.RaisePropertyReceived(ref this.uIntValue, value);
        }
        public ushort? UShortValue
        {
            get { RaisePropertyCalled(uShortValue); return uShortValue; }
            set => this.RaisePropertyReceived(ref this.uShortValue, value);
        }
        public ulong? ULongValue
        {
            get { RaisePropertyCalled(uLongValue); return uLongValue; }
            set => this.RaisePropertyReceived(ref this.uLongValue, value);
        }
        public string? StringValue
        {
            get { RaisePropertyCalled(stringValue); return stringValue; }
            set => this.RaisePropertyReceived(ref this.stringValue, value);
        }

        // Method to convert back to the original value
        public object ToOriginalValue()
        {
            return RuntimeTypeConverter.FromType(Type) switch
            {
                RuntimeType.Int => IntValue.Value,
                RuntimeType.Float => FloatValue.Value,
                RuntimeType.Double => DoubleValue.Value,
                RuntimeType.Decimal => DecimalValue.Value,
                RuntimeType.Bool => BoolValue.Value,
                RuntimeType.Char => CharValue.Value,
                RuntimeType.Byte => ByteValue.Value,
                RuntimeType.SByte => SByteValue.Value,
                RuntimeType.Short => ShortValue.Value,
                RuntimeType.Long => LongValue.Value,
                RuntimeType.UInt => UIntValue.Value,
                RuntimeType.UShort => UShortValue.Value,
                RuntimeType.ULong => ULongValue.Value,
                RuntimeType.String => StringValue,
                RuntimeType.Object => Value,
                _ => throw new InvalidOperationException("No active property to convert.")
            };
        }

        // Explicit conversion operators for nullable types
        public static explicit operator int?(NodeViewModel d)
        {
            return d.IntValue;
        }

        public static explicit operator float?(NodeViewModel d)
        {
            return d.FloatValue;
        }

        public static explicit operator double?(NodeViewModel d)
        {
            return d.DoubleValue;
        }

        public static explicit operator decimal?(NodeViewModel d)
        {
            return d.DecimalValue;
        }

        public static explicit operator bool?(NodeViewModel d)
        {
            return d.BoolValue;
        }

        public static explicit operator char?(NodeViewModel d)
        {
            return d.CharValue;
        }

        public static explicit operator byte?(NodeViewModel d)
        {
            return d.ByteValue;
        }

        public static explicit operator sbyte?(NodeViewModel d)
        {
            return d.SByteValue;
        }

        public static explicit operator short?(NodeViewModel d)
        {
            return d.ShortValue;
        }

        public static explicit operator long?(NodeViewModel d)
        {
            return d.LongValue;
        }

        public static explicit operator uint?(NodeViewModel d)
        {
            return d.UIntValue;
        }

        public static explicit operator ushort?(NodeViewModel d)
        {
            return d.UShortValue;
        }

        public static explicit operator ulong?(NodeViewModel d)
        {
            return d.ULongValue;
        }

        public static explicit operator string?(NodeViewModel d)
        {
            return d.StringValue;
        }


        // Explicit conversion operators for non-nullable types
        public static explicit operator int(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.IntValue.HasValue)
                return d.IntValue.Value;
            throw new InvalidOperationException("IntValue is not the active property or is null.");
        }

        public static explicit operator float(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.FloatValue.HasValue)
                return d.FloatValue.Value;
            throw new InvalidOperationException("FloatValue is not the active property or is null.");
        }

        public static explicit operator double(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.DoubleValue.HasValue)
                return d.DoubleValue.Value;
            throw new InvalidOperationException("DoubleValue is not the active property or is null.");
        }

        public static explicit operator decimal(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.DecimalValue.HasValue)
                return d.DecimalValue.Value;
            throw new InvalidOperationException("DecimalValue is not the active property or is null.");
        }

        public static explicit operator bool(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.BoolValue.HasValue)
                return d.BoolValue.Value;
            throw new InvalidOperationException("BoolValue is not the active property or is null.");
        }

        public static explicit operator char(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.CharValue.HasValue)
                return d.CharValue.Value;
            throw new InvalidOperationException("CharValue is not the active property or is null.");
        }

        public static explicit operator byte(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.ByteValue.HasValue)
                return d.ByteValue.Value;
            throw new InvalidOperationException("ByteValue is not the active property or is null.");
        }

        public static explicit operator sbyte(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.SByteValue.HasValue)
                return d.SByteValue.Value;
            throw new InvalidOperationException("SByteValue is not the active property or is null.");
        }

        public static explicit operator short(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.ShortValue.HasValue)
                return d.ShortValue.Value;
            throw new InvalidOperationException("ShortValue is not the active property or is null.");
        }

        public static explicit operator long(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.LongValue.HasValue)
                return d.LongValue.Value;
            throw new InvalidOperationException("LongValue is not the active property or is null.");
        }

        public static explicit operator uint(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.UIntValue.HasValue)
                return d.UIntValue.Value;
            throw new InvalidOperationException("UIntValue is not the active property or is null.");
        }

        public static explicit operator ushort(NodeViewModel d)
        {
            if (d.Type == typeof(int) && d.UShortValue.HasValue)
                return d.UShortValue.Value;
            throw new InvalidOperationException("UShortValue is not the active property or is null.");
        }

        public static explicit operator ulong(NodeViewModel d)
        {
            if (d.Type == typeof(ulong) && d.ULongValue.HasValue)
                return d.ULongValue.Value;
            throw new InvalidOperationException("ULongValue is not the active property or is null.");
        }
    }
}