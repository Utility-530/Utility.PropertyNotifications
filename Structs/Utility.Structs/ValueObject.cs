using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;



namespace Utility.Structs
{


    public readonly struct ValueObject
    {
        public int? IntValue { get; }
        public float? FloatValue { get; }
        public double? DoubleValue { get; }
        public decimal? DecimalValue { get; }
        public bool? BoolValue { get; }
        public char? CharValue { get; }
        public byte? ByteValue { get; }
        public sbyte? SByteValue { get; }
        public short? ShortValue { get; }
        public long? LongValue { get; }
        public uint? UIntValue { get; }
        public ushort? UShortValue { get; }
        public ulong? ULongValue { get; }

        public string ActiveProperty { get; }

        // Constructor
        private ValueObject(int? intValue, float? floatValue, double? doubleValue,
                                decimal? decimalValue, bool? boolValue, char? charValue,
                                byte? byteValue, sbyte? sbyteValue, short? shortValue,
                                long? longValue, uint? uintValue, ushort? ushortValue,
                                ulong? ulongValue, string activeProperty)
        {
            IntValue = intValue;
            FloatValue = floatValue;
            DoubleValue = doubleValue;
            DecimalValue = decimalValue;
            BoolValue = boolValue;
            CharValue = charValue;
            ByteValue = byteValue;
            SByteValue = sbyteValue;
            ShortValue = shortValue;
            LongValue = longValue;
            UIntValue = uintValue;
            UShortValue = ushortValue;
            ULongValue = ulongValue;
            ActiveProperty = activeProperty;
        }

        // Method to convert any value into an instance of this struct
        public static ValueObject FromValue(object value)
        {
            return value switch
            {
                int intValue => new ValueObject(intValue, null, null, null, null, null, null, null, null, null, null, null, null, nameof(IntValue)),
                float floatValue => new ValueObject(null, floatValue, null, null, null, null, null, null, null, null, null, null, null, nameof(FloatValue)),
                double doubleValue => new ValueObject(null, null, doubleValue, null, null, null, null, null, null, null, null, null, null, nameof(DoubleValue)),
                decimal decimalValue => new ValueObject(null, null, null, decimalValue, null, null, null, null, null, null, null, null, null, nameof(DecimalValue)),
                bool boolValue => new ValueObject(null, null, null, null, boolValue, null, null, null, null, null, null, null, null, nameof(BoolValue)),
                char charValue => new ValueObject(null, null, null, null, null, charValue, null, null, null, null, null, null, null, nameof(CharValue)),
                byte byteValue => new ValueObject(null, null, null, null, null, null, byteValue, null, null, null, null, null, null, nameof(ByteValue)),
                sbyte sbyteValue => new ValueObject(null, null, null, null, null, null, null, sbyteValue, null, null, null, null, null, nameof(SByteValue)),
                short shortValue => new ValueObject(null, null, null, null, null, null, null, null, shortValue, null, null, null, null, nameof(ShortValue)),
                long longValue => new ValueObject(null, null, null, null, null, null, null, null, null, longValue, null, null, null, nameof(LongValue)),
                uint uintValue => new ValueObject(null, null, null, null, null, null, null, null, null, null, uintValue, null, null, nameof(UIntValue)),
                ushort ushortValue => new ValueObject(null, null, null, null, null, null, null, null, null, null, null, ushortValue, null, nameof(UShortValue)),
                ulong ulongValue => new ValueObject(null, null, null, null, null, null, null, null, null, null, null, null, ulongValue, nameof(ULongValue)),
                _ => throw new ArgumentException("Unsupported value type.")

            };

        }



        // Method to convert back to the original value
        public object ToOriginalValue()
        {
            return ActiveProperty switch
            {
                nameof(IntValue) => IntValue.Value,
                nameof(FloatValue) => FloatValue.Value,
                nameof(DoubleValue) => DoubleValue.Value,
                nameof(DecimalValue) => DecimalValue.Value,
                nameof(BoolValue) => BoolValue.Value,
                nameof(CharValue) => CharValue.Value,
                nameof(ByteValue) => ByteValue.Value,
                nameof(SByteValue) => SByteValue.Value,
                nameof(ShortValue) => ShortValue.Value,
                nameof(LongValue) => LongValue.Value,
                nameof(UIntValue) => UIntValue.Value,
                nameof(UShortValue) => UShortValue.Value,
                nameof(ULongValue) => ULongValue.Value,
                _ => throw new InvalidOperationException("No active property to convert.")
            };
        }

        // Explicit conversion operators for nullable types
        public static explicit operator int?(ValueObject d)
        {
            return d.IntValue;
        }

        public static explicit operator float?(ValueObject d)
        {
            return d.FloatValue;
        }

        public static explicit operator double?(ValueObject d)
        {
            return d.DoubleValue;
        }

        public static explicit operator decimal?(ValueObject d)
        {
            return d.DecimalValue;
        }

        public static explicit operator bool?(ValueObject d)
        {
            return d.BoolValue;
        }

        public static explicit operator char?(ValueObject d)
        {
            return d.CharValue;
        }

        public static explicit operator byte?(ValueObject d)
        {
            return d.ByteValue;
        }

        public static explicit operator sbyte?(ValueObject d)
        {
            return d.SByteValue;
        }

        public static explicit operator short?(ValueObject d)
        {
            return d.ShortValue;
        }

        public static explicit operator long?(ValueObject d)
        {
            return d.LongValue;
        }

        public static explicit operator uint?(ValueObject d)
        {
            return d.UIntValue;
        }

        public static explicit operator ushort?(ValueObject d)
        {
            return d.UShortValue;
        }

        public static explicit operator ulong?(ValueObject d)
        {
            return d.ULongValue;
        }


        // Explicit conversion operators for non-nullable types
        public static explicit operator int(ValueObject d)
        {
            if (d.ActiveProperty == nameof(IntValue) && d.IntValue.HasValue)
                return d.IntValue.Value;
            throw new InvalidOperationException("IntValue is not the active property or is null.");
        }

        public static explicit operator float(ValueObject d)
        {
            if (d.ActiveProperty == nameof(FloatValue) && d.FloatValue.HasValue)
                return d.FloatValue.Value;
            throw new InvalidOperationException("FloatValue is not the active property or is null.");
        }

        public static explicit operator double(ValueObject d)
        {
            if (d.ActiveProperty == nameof(DoubleValue) && d.DoubleValue.HasValue)
                return d.DoubleValue.Value;
            throw new InvalidOperationException("DoubleValue is not the active property or is null.");
        }

        public static explicit operator decimal(ValueObject d)
        {
            if (d.ActiveProperty == nameof(DecimalValue) && d.DecimalValue.HasValue)
                return d.DecimalValue.Value;
            throw new InvalidOperationException("DecimalValue is not the active property or is null.");
        }

        public static explicit operator bool(ValueObject d)
        {
            if (d.ActiveProperty == nameof(BoolValue) && d.BoolValue.HasValue)
                return d.BoolValue.Value;
            throw new InvalidOperationException("BoolValue is not the active property or is null.");
        }

        public static explicit operator char(ValueObject d)
        {
            if (d.ActiveProperty == nameof(CharValue) && d.CharValue.HasValue)
                return d.CharValue.Value;
            throw new InvalidOperationException("CharValue is not the active property or is null.");
        }

        public static explicit operator byte(ValueObject d)
        {
            if (d.ActiveProperty == nameof(ByteValue) && d.ByteValue.HasValue)
                return d.ByteValue.Value;
            throw new InvalidOperationException("ByteValue is not the active property or is null.");
        }

        public static explicit operator sbyte(ValueObject d)
        {
            if (d.ActiveProperty == nameof(SByteValue) && d.SByteValue.HasValue)
                return d.SByteValue.Value;
            throw new InvalidOperationException("SByteValue is not the active property or is null.");
        }

        public static explicit operator short(ValueObject d)
        {
            if (d.ActiveProperty == nameof(ShortValue) && d.ShortValue.HasValue)
                return d.ShortValue.Value;
            throw new InvalidOperationException("ShortValue is not the active property or is null.");
        }

        public static explicit operator long(ValueObject d)
        {
            if (d.ActiveProperty == nameof(LongValue) && d.LongValue.HasValue)
                return d.LongValue.Value;
            throw new InvalidOperationException("LongValue is not the active property or is null.");
        }

        public static explicit operator uint(ValueObject d)
        {
            if (d.ActiveProperty == nameof(UIntValue) && d.UIntValue.HasValue)
                return d.UIntValue.Value;
            throw new InvalidOperationException("UIntValue is not the active property or is null.");
        }

        public static explicit operator ushort(ValueObject d)
        {
            if (d.ActiveProperty == nameof(UShortValue) && d.UShortValue.HasValue)
                return d.UShortValue.Value;
            throw new InvalidOperationException("UShortValue is not the active property or is null.");
        }

        public static explicit operator ulong(ValueObject d)
        {
            if (d.ActiveProperty == nameof(ULongValue) && d.ULongValue.HasValue)
                return d.ULongValue.Value;
            throw new InvalidOperationException("ULongValue is not the active property or is null.");
        }
    }
}