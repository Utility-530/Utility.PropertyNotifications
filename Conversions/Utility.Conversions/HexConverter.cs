using System.Text;

namespace Utility.Conversions
{
    public static class HexConverter
    {
        private const string _hexaChars = "0123456789ABCDEF";



        public static string ToHexa(this byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            return ToHexa(bytes, 0, bytes.Length);
        }

        public static string ToHexa(this byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            if (offset < 0)
            {
                throw new ArgumentException(null, "offset");
            }

            if (count < 0)
            {
                throw new ArgumentException(null, "count");
            }

            if (offset >= bytes.Length)
            {
                return string.Empty;
            }

            count = Math.Min(count, bytes.Length - offset);

            StringBuilder sb = new StringBuilder(count * 2);
            for (int i = offset; i < (offset + count); i++)
            {
                sb.Append(_hexaChars[bytes[i] / 16]);
                sb.Append(_hexaChars[bytes[i] % 16]);
            }
            return sb.ToString();
        }
    }
}