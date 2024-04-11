using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utility.Interfaces.NonGeneric;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utility.Keys
{
    public record Key 
    {
        public static implicit operator string(Key d) => d.ToString();
        //public static explicit operator Digit(byte b) => new Digit(b);


    }
}