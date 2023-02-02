using System;
using System.Collections.Generic;
using System.Web;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;


namespace Utility.Structs.FileSystem
{
    /// <summary>
    /// Represents the size of a file.  
    /// </summary>
    /// <remarks>
    /// <a href="https://github.com/StevePotter/FileSize/blob/master/FileSize.cs"></a>
    /// This is super useful when converting between bytes, megabytes, gigabytes, etc. \n 
    /// Provides nice parsing for values like '2 GB' or '1.54mb'. \n 
    /// Also has nice formatting in its ToString method.
    /// </remarks>
    [TypeConverter(typeof(FileSizeTypeConverter))]
    public struct FileSize : IComparable<FileSize>, IEquatable<FileSize>
    {
        //contants are public in case anyone wants to do their own math
        public const double BYTES_PER_BIT = 0.125;
        public const long BYTES_PER_KILABYTE = 1024;
        public const long BYTES_PER_MEGABYTE = 1048576;
        public const long BYTES_PER_GIGABYTE = 1073741824;
        public const long BYTES_PER_TERABYTE = 1099511627776;


        /// <summary>
        /// Creates a FileSize object with the given number of bytes.
        /// </summary>
        public FileSize(long bytes)
        {
            Bytes = bytes;
        }

        /// <summary>
        /// Creates a FileSize object with the given number of bytes.
        /// </summary>
        /// <param name="bytes"></param>
        public FileSize(int bytes)
        {
            Bytes = bytes;
        }

        /// <summary>
        /// Creates a FileSize object with the given string to parse, such as "2mb, "2 MB", "212.5 tb", "21B", into the number of bytes.  Units can be "", "gb", "kb", "mb", "tb".  Casing and spaces make no difference.  No unit assumes bytes.  Will thrown an exception for malformed input.
        /// </summary>
        /// <param name="value"></param>
        public FileSize(string value)
        {
            Bytes = Parse(value).Bytes;
        }

        /// <summary>
        /// At the end of the day, we store only the number of bytes for the file size. 
        /// </summary>

        /// <summary>
        /// The total number of bits.
        /// </summary>
        public long Bits
        {
            get => Convert.ToInt64(Bytes / BYTES_PER_BIT);
            set => Bytes = Convert.ToInt64(Math.Round(value * BYTES_PER_BIT));
        }

        /// <summary>
        /// The total number of bytes.
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// The total number of kilabytes.
        /// </summary>
        public double KilaBytes
        {
            get => Bytes / (double)BYTES_PER_KILABYTE;
            set => Bytes = Convert.ToInt64(Math.Round(value * BYTES_PER_KILABYTE));
        }


        /// <summary>
        /// The total number of megabytes.
        /// </summary>
        public double MegaBytes
        {
            get => Bytes / (double)BYTES_PER_MEGABYTE;
            set => Bytes = Convert.ToInt64(Math.Round(value * BYTES_PER_MEGABYTE));
        }


        /// <summary>
        /// The total number of gigabytes.
        /// </summary>
        public double GigaBytes
        {
            get => Bytes / (double)BYTES_PER_GIGABYTE;
            set => Bytes = Convert.ToInt64(Math.Round(value * BYTES_PER_GIGABYTE));
        }

        /// <summary>
        /// The total number of terabytes.
        /// </summary>
        public double TeraBytes
        {
            get => Bytes / (double)BYTES_PER_TERABYTE;
            set => Bytes = Convert.ToInt64(Math.Round(value * BYTES_PER_TERABYTE));
        }


        /// <summary>
        /// Creates a friendly string to show the value.  If it's greater than 1 terabyte, it'll show in terabytes, and likewise until it gets down to bits.  Possible return values are "1.23 TB, 15.01 GB, 450.59 MB, 4509 KB, 3123434 B, or 24123412341234 bits".  Rounding is done in the case of decimals, to 2 digits.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var absoluteBytes = Math.Abs(Bytes);
            if (absoluteBytes == 0)
                return "0 KB";
            if (absoluteBytes >= BYTES_PER_TERABYTE)
                return Math.Round(TeraBytes, 2) + " TB";
            if (absoluteBytes >= BYTES_PER_GIGABYTE)
                return Math.Round(GigaBytes, 2) + " GB";
            if (absoluteBytes >= BYTES_PER_MEGABYTE)
                return Math.Round(MegaBytes, 2) + " MB";
            if (absoluteBytes >= BYTES_PER_KILABYTE)
                return Math.Round(KilaBytes, 2) + " KB";
            if (absoluteBytes >= 1)
                return Bytes.ToString(NumberFormatInfo.InvariantInfo) + " B";

            return Bits.ToString(NumberFormatInfo.InvariantInfo) + " bits";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is FileSize))
                return false;
            return (FileSize)obj == this;
        }

        public override int GetHashCode()
        {
            return Bytes.GetHashCode();
        }


        /// <summary>
        /// Creates a FileSize object with the given string to parse, such as "2mb, "2 MB", "212.5 tb", "21B", into the number of bytes.  Units can be "", "gb", "kb", "mb", "tb".  Casing and spaces make no difference.  No unit assumes bytes.  Will thrown an exception for malformed input.
        /// </summary>
        public static FileSize Parse(string value)
        {
            FileSize fileSize = new FileSize();
            string number = "";
            string unit = "";
            foreach (var c in value.ToCharArray())
            {
                if (number.Length > 0 && char.IsLetter(c))
                    unit += c;
                else if (char.IsDigit(c) || c == '.')
                    number += c;
            }

            double val = !string.IsNullOrEmpty(number) ? double.Parse(number, NumberFormatInfo.InvariantInfo) : 0;
            unit = unit.Trim().ToLowerInvariant();
            switch (unit)
            {
                case "":
                case "b":
                    fileSize.Bytes = (long)Math.Round(val);
                    break;
                case "kb":
                    fileSize.KilaBytes = val;
                    break;
                case "mb":
                    fileSize.MegaBytes = val;
                    break;
                case "gb":
                    fileSize.GigaBytes = val;
                    break;
                case "tb":
                    fileSize.TeraBytes = val;
                    break;
                default:
                    throw new ArgumentException("Unit " + unit + " not supported.");
            }
            return fileSize;
        }

        /// <summary>
        /// Attempts to create a FileSize object with the given string to parse, such as "2mb, "2 MB", "212.5 tb", "21B", into the number of bytes.  Units can be "", "gb", "kb", "mb", "tb".  Casing and spaces make no difference.  No unit assumes bytes.
        /// </summary>
        public static FileSize? TryParse(string value)
        {
            FileSize fileSize = new FileSize();
            string number = "";
            string unit = "";
            foreach (var c in value.ToCharArray())
            {
                if (number.Length > 0 && char.IsLetter(c))
                    unit += c;
                else if (char.IsDigit(c) || c == '.')
                    number += c;
            }

            double val = !string.IsNullOrEmpty(number) ? double.Parse(number, NumberFormatInfo.InvariantInfo) : 0;
            unit = unit.Trim().ToLowerInvariant();
            switch (unit)
            {
                case "":
                case "b":
                    fileSize.Bytes = (long)Math.Round(val);
                    break;
                case "kb":
                    fileSize.KilaBytes = val;
                    break;
                case "mb":
                    fileSize.MegaBytes = val;
                    break;
                case "gb":
                    fileSize.GigaBytes = val;
                    break;
                case "tb":
                    fileSize.TeraBytes = val;
                    break;
                default:
                    return new FileSize?();
            }
            return fileSize;

        }


        public static FileSize operator -(FileSize t1, FileSize t2) => new FileSize(t1.Bytes - t2.Bytes);

        public static FileSize operator +(FileSize t1, FileSize t2) => new FileSize(t1.Bytes + t2.Bytes);

        public static bool operator ==(FileSize t1, FileSize t2) => t1.Bytes == t2.Bytes;

        public static bool operator !=(FileSize t1, FileSize t2) => t1.Bytes != t2.Bytes;

        public static bool operator <(FileSize t1, FileSize t2) => t1.Bytes < t2.Bytes;

        public static bool operator <=(FileSize t1, FileSize t2) => t1.Bytes <= t2.Bytes;

        public static bool operator >(FileSize t1, FileSize t2) => t1.Bytes > t2.Bytes;

        public static bool operator >=(FileSize t1, FileSize t2) => t1.Bytes >= t2.Bytes;


        public int CompareTo(FileSize other)
        {
            long num = other.Bytes;
            if (Bytes > num)
            {
                return 1;
            }
            if (Bytes < num)
            {
                return -1;
            }
            return 0;
        }

        public bool Equals(FileSize other) => Bytes == other.Bytes;
    }

    /// <summary>
    /// Type converter for the FileSize object.
    /// </summary>
    public class FileSizeTypeConverter : UriTypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == null
                ? throw new ArgumentNullException("sourceType")
                : sourceType == typeof(string) || sourceType == typeof(long) || sourceType == typeof(int) || sourceType == typeof(FileSize) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == null
                ? throw new ArgumentNullException("destinationType")
                : destinationType == typeof(string) || destinationType == typeof(long) || destinationType == typeof(FileSize) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            value == null
                ? new FileSize()
                : value is string asString
                ? new FileSize(asString)
                : value is FileSize size
                ? new FileSize(size.Bytes)
                : value is long int1
                ? new FileSize(int1)
                : value is int @int ? new FileSize(@int) : base.ConvertFrom(context, culture, value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is FileSize size2)
            {
                return new InstanceDescriptor(typeof(Uri).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(long) }, null), new object[] { size2.Bytes });
            }
            if (destinationType == typeof(string) && value is FileSize size)
            {
                return size.ToString();
            }
            return destinationType == typeof(FileSize) && value is FileSize size1
                ? new FileSize(size1.Bytes)
                : base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return value is string strValue ?
                FileSize.TryParse(strValue).HasValue :
                value is FileSize || value is long || value is int;
        }
    }
}