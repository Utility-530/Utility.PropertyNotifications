using System;
using System.Security.Cryptography;
using System.Text;

namespace Utility.WPF.Colours
{
    public class ColoursHelpers
    {
        public static string PastelColorForString(string s)
        {
            uint hashCode = BitConverter.ToUInt32(SHA1.HashData(Encoding.UTF8.GetBytes(s)), 0);

            return "#" +
                ((((hashCode >> 24) & 0xFF) + 255) / 2).ToString("X") +
                ((((hashCode >> 16) & 0xFF) + 255) / 2).ToString("X") +
                ((((hashCode >> 08) & 0xFF) + 255) / 2).ToString("X");
        }
    }
}