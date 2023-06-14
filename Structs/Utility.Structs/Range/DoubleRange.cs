// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Utility.Structs
{
    using System;

    /// <summary>
    ///   Represents a double range with Minimum and Maximum values.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class represents a double range with inclusive limits, where
    ///   both Minimum and Maximum values of the range are included into it.
    ///   Mathematical notation of such range is <b>[Min, Max]</b>.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // create [0.25, 1.5] range
    /// var range1 = new DoubleRange(0.25, 1.5);
    /// 
    /// // create [1.00, 2.25] range
    /// var range2 = new DoubleRange(1.00, 2.25);
    /// 
    /// // check if values is inside of the first range
    /// if (range1.IsInside(0.75))
    /// {
    ///     // ...
    /// }
    /// 
    /// // check if the second range is inside of the first range
    /// if (range1.IsInside(range2))
    /// {
    ///     // ...
    /// }
    /// 
    /// // check if two ranges overlap
    /// if (range1.IsOverlapping(range2))
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="ByteRange"/>
    /// <seealso cref="IntRange"/>
    /// <seealso cref="Range"/>
    /// 
    [Serializable]
    public readonly struct DoubleRange : /*IRange<double>, */IEquatable<DoubleRange>
    {

        /// <summary>
        ///   Minimum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        ///   Represents Minimum value (left side limit) of the range [<b>Min</b>, Max].
        /// </remarks>
        /// 
        public double Min        {            get;        }

        /// <summary>
        ///   Maximum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        ///   Represents Maximum value (right side limit) of the range [Min, <b>Max</b>].
        /// </remarks>
        /// 
        public double Max        {            get;        }

        /// <summary>
        ///   Gets the length of the range, defined as (Max - Min).
        /// </summary>
        /// 
        public double Length
        {
            get { return Max - Min; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DoubleRange"/> class.
        /// </summary>
        /// 
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        /// 
        public DoubleRange(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        ///   Check if the specified value is inside of the range.
        /// </summary>
        /// 
        /// <param name="x">Value to check.</param>
        /// 
        /// <returns>
        ///   <b>True</b> if the specified value is inside of the range or <b>false</b> otherwise.
        /// </returns>
        /// 
        public bool IsInside(double x)
        {
            return ((x >= Min) && (x <= Max));
        }

        /// <summary>
        ///   Check if the specified range is inside of the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check.</param>
        /// 
        /// <returns>
        ///   <b>True</b> if the specified range is inside of the range or <b>false</b> otherwise.
        /// </returns>
        /// 
        public bool IsInside(DoubleRange range)
        {
            return ((IsInside(range.Min)) && (IsInside(range.Max)));
        }

        /// <summary>
        ///   Check if the specified range overlaps with the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check for overlapping.</param>
        /// 
        /// <returns>
        ///   <b>True</b> if the specified range overlaps with the range or <b>false</b> otherwise.
        /// </returns>
        /// 
        public bool IsOverlapping(DoubleRange range)
        {
            return ((IsInside(range.Min)) || (IsInside(range.Max)) ||
                     (range.IsInside(Min)) || (range.IsInside(Max)));
        }

        /// <summary>
        ///   Computes the intersection between two ranges.
        /// </summary>
        /// 
        /// <param name="range">The second range for which the intersection should be calculated.</param>
        /// 
        /// <returns>An new <see cref="IntRange"/> structure containing the intersection
        /// between this range and the <paramref name="range"/> given as argument.</returns>
        /// 
        public DoubleRange Intersection(DoubleRange range)
        {
            return new DoubleRange(System.Math.Max(this.Min, range.Min), System.Math.Min(this.Max, range.Max));
        }

        /// <summary>
        ///   DeterMines whether two instances are equal.
        /// </summary>
        /// 
        public static bool operator ==(DoubleRange range1, DoubleRange range2)
        {
            return ((range1.Min == range2.Min) && (range1.Max == range2.Max));
        }

        /// <summary>
        ///   DeterMines whether two instances are not equal.
        /// </summary>
        /// 
        public static bool operator !=(DoubleRange range1, DoubleRange range2)
        {
            return ((range1.Min != range2.Min) || (range1.Max != range2.Max));
        }

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// 
        /// <param name="other">An object to compare with this object.</param>
        /// 
        /// <returns>
        ///   true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// 
        public bool Equals(DoubleRange other)
        {
            return this == other;
        }

        /// <summary>
        ///   DeterMines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// 
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public override bool Equals(object obj)
        {
            return (obj is DoubleRange) ? (this == (DoubleRange)obj) : false;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Min.GetHashCode();
                hash = hash * 31 + Max.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return String.Format("[{0}, {1}]", Min, Max);
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("[{0}, {1}]",
                Min.ToString(format, formatProvider),
                Max.ToString(format, formatProvider));
        }



        /// <summary>
        ///   Converts this double-precision range into an <see cref="IntRange"/>.
        /// </summary>
        /// 
        /// <param name="provideInnerRange">
        ///   Specifies if inner integer range must be returned or outer range.</param>
        /// 
        /// <returns>Returns integer version of the range.</returns>
        /// 
        /// <remarks>
        ///   If <paramref name="provideInnerRange"/> is set to <see langword="true"/>, then the
        ///   returned integer range will always fit inside of the current single precision range.
        ///   If it is set to <see langword="false"/>, then current single precision range will always
        ///   fit into the returned integer range.
        /// </remarks>
        ///
        public IntRange ToIntRange(bool provideInnerRange)
        {
            int iMin, iMax;

            if (provideInnerRange)
            {
                iMin = (int)System.Math.Ceiling(Min);
                iMax = (int)System.Math.Floor(Max);
            }
            else
            {
                iMin = (int)System.Math.Floor(Min);
                iMax = (int)System.Math.Ceiling(Max);
            }

            return new IntRange(iMin, iMax);
        }

        /// <summary>
        /// Converts this <see cref="DoubleRange"/> to a <see cref="T:System.Double[]"/> of length 2 (using new [] { Min, Max }).
        /// </summary>
        /// 
        /// <returns>The result of the conversion.</returns>
        /// 
        public double[] ToArray()
        {
            return new[] { Min, Max };
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DoubleRange"/> to <see cref="T:System.Double[]"/>.
        /// </summary>
        /// 
        /// <param name="range">The range.</param>
        /// 
        /// <returns>The result of the conversion.</returns>
        /// 
        public static implicit operator double[](DoubleRange range)
        {
            return range.ToArray();
        }

    }
}