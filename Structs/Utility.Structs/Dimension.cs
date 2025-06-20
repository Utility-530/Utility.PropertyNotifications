using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Utility.Enums;


namespace Utility.Structs
{

    /// <summary>
    /// Holds the width or height of a <see cref="Grid"/>'s column and row definitions.
    /// </summary>
#if !BUILDTASK
    public
#endif
    struct Dimension : IEquatable<Dimension>
    {
        private DimensionUnitType _type;
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dimension"/> struct.
        /// </summary>
        /// <param name="value">The size of the Dimension in device independent pixels.</param>
        public Dimension(double value)
            : this(value, DimensionUnitType.Pixel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dimension"/> struct.
        /// </summary>
        /// <param name="value">The size of the Dimension.</param>
        /// <param name="type">The unit of the Dimension.</param>
        public Dimension(double value, DimensionUnitType type)
        {
            if (value < 0 || double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException("Invalid value", nameof(value));
            }

            if (type < DimensionUnitType.Auto || type > DimensionUnitType.Star)
            {
                throw new ArgumentException("Invalid value", nameof(type));
            }

            _type = type;
            _value = value;
        }

        /// <summary>
        /// Gets an instance of <see cref="Dimension"/> that indicates that a row or column should
        /// auto-size to fit its content.
        /// </summary>
        public static Dimension Auto => new (0, DimensionUnitType.Auto);

        /// <summary>
        /// Gets an instance of <see cref="Dimension"/> that indicates that a row or column should
        /// fill its content.
        /// </summary>
        public static Dimension Star => new (1, DimensionUnitType.Star);

        /// <summary>
        /// Gets the unit of the <see cref="Dimension"/>.
        /// </summary>
        public DimensionUnitType UnitType { readonly get => _type; set { _type = value; } }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="Dimension"/> has a <see cref="UnitType"/> of Pixel.
        /// </summary>
        public readonly bool IsAbsolute => _type == DimensionUnitType.Pixel;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="Dimension"/> has a <see cref="UnitType"/> of Auto.
        /// </summary>
        public readonly bool IsAuto => _type == DimensionUnitType.Auto;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="Dimension"/> has a <see cref="UnitType"/> of Star.
        /// </summary>
        public readonly bool IsStar => _type == DimensionUnitType.Star;

        /// <summary>
        /// Gets the length.
        /// </summary>
        public double Value { readonly get => _value; set => _value = value; }

        /// <summary>
        /// Compares two Dimension structures for equality.
        /// </summary>
        /// <param name="a">The first Dimension.</param>
        /// <param name="b">The second Dimension.</param>
        /// <returns>True if the structures are equal, otherwise false.</returns>
        public static bool operator ==(Dimension a, Dimension b)
        {
            return a.IsAuto && b.IsAuto || a._value == b._value && a._type == b._type;
        }

        /// <summary>
        /// Compares two Dimension structures for inequality.
        /// </summary>
        /// <param name="gl1">The first Dimension.</param>
        /// <param name="gl2">The first Dimension.</param>
        /// <returns>True if the structures are unequal, otherwise false.</returns>
        public static bool operator !=(Dimension gl1, Dimension gl2)
        {
            return !(gl1 == gl2);
        }

        /// <summary>
        /// Determines whether the <see cref="Dimension"/> is equal to the specified object.
        /// </summary>
        /// <param name="o">The object with which to test equality.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public override readonly bool Equals(object o)
        {
            return o switch
            {
                null => false,
                not Dimension => false,
                _ => this == (Dimension)o,
            };
        }

        /// <summary>
        /// Compares two Dimension structures for equality.
        /// </summary>
        /// <param name="Dimension">The structure with which to test equality.</param>
        /// <returns>True if the structures are equal, otherwise false.</returns>
        public readonly bool Equals(Dimension Dimension) => this == Dimension;

        /// <summary>
        /// Gets a hash code for the Dimension.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode() ^ _type.GetHashCode();
        }

        /// <summary>
        /// Gets a string representation of the <see cref="Dimension"/>.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (IsAuto)
            {
                return "Auto";
            }

            string s = _value.ToString();
            return IsStar ? s + "*" : s;
        }

        /// <summary>
        /// Parses a string to return a <see cref="Dimension"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The <see cref="Dimension"/>.</returns>
        public static Dimension Parse(string s)
        {
            s = s.ToUpperInvariant();

            if (s == "AUTO")
            {
                return Auto;
            }
            else if (s.EndsWith("*"))
            {
                var valueString = s.Substring(0, s.Length - 1).Trim();
                var value = valueString.Length > 0 ? double.Parse(valueString, CultureInfo.InvariantCulture) : 1;
                return new Dimension(value, DimensionUnitType.Star);
            }
            else
            {
                var value = double.Parse(s, CultureInfo.InvariantCulture);
                return new Dimension(value, DimensionUnitType.Pixel);
            }
        }

        public static explicit operator Dimension(string d) => Parse(d);

        /// <summary>
        /// Parses a string to return a collection of <see cref="Dimension"/>s.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The <see cref="Dimension"/>.</returns>
        public static IEnumerable<Dimension> ParseLengths(string s)
        {
            return s.Split(',').Select(s => Parse(s));
        }
    }
}