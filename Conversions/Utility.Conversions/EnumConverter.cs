using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Utility.Conversions;

namespace Utilities
{
    public static class EnumConverter
    {

        //public static string ConcatenateCollection(IEnumerable collection, string expression, string separator)
        //{
        //    return ConcatenateCollection(collection, expression, separator, null);
        //}

        public static object EnumToObject(Type enumType, object value)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            if (!enumType.IsEnum)
            {
                throw new ArgumentException(null, "enumType");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Type underlyingType = Enum.GetUnderlyingType(enumType);
            if (underlyingType == typeof(long))
            {
                return Enum.ToObject(enumType, ConversionHelper.ChangeType<long>(value));
            }

            if (underlyingType == typeof(ulong))
            {
                return Enum.ToObject(enumType, ConversionHelper.ChangeType<ulong>(value));
            }

            if (underlyingType == typeof(int))
            {
                return Enum.ToObject(enumType, ConversionHelper.ChangeType<int>(value));
            }

            if ((underlyingType == typeof(uint)))
            {
                return Enum.ToObject(enumType, ConversionHelper.ChangeType<uint>(value));
            }

            if (underlyingType == typeof(short))
            {
                return Enum.ToObject(enumType, ConversionHelper.ChangeType<short>(value));
            }

            if (underlyingType == typeof(ushort))
            {
                return Enum.ToObject(enumType, ConversionHelper.ChangeType<ushort>(value));
            }

            if (underlyingType == typeof(byte))
            {
                return Enum.ToObject(enumType, ConversionHelper.ChangeType<byte>(value));
            }

            if (underlyingType == typeof(sbyte))
            {
                return Enum.ToObject(enumType, ConversionHelper.ChangeType<sbyte>(value));
            }

            throw new ArgumentException(null, "enumType");
        }

        public static ulong EnumToUInt64(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            TypeCode typeCode = Convert.GetTypeCode(value);
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return Convert.ToUInt64(value, CultureInfo.InvariantCulture);

                //case TypeCode.String:
                default:
                    return ConversionHelper.ChangeType<ulong>(value);
            }
        }

   
        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}