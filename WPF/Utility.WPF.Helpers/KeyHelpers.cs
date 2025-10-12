using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Utility.WPF.Helpers
{
    public static class KeyHelpers
    {
        public static decimal ExtractDigit(this Key key)
        {
            return key switch
            {
                Key.D0 or Key.NumPad0 => 0M,
                Key.D1 or Key.NumPad1 => 1M,
                Key.D2 or Key.NumPad2 => 2M,
                Key.D3 or Key.NumPad3 => 3M,
                Key.D4 or Key.NumPad4 => 4M,
                Key.D5 or Key.NumPad5 => 5M,
                Key.D6 or Key.NumPad6 => 6M,
                Key.D7 or Key.NumPad7 => 7M,
                Key.D8 or Key.NumPad8 => 8M,
                Key.D9 or Key.NumPad9 => 9M,
                _ => throw new ArgumentOutOfRangeException("Invalid key: " + key.ToString()),
            };
        }

        public static bool IsNumeric(this Key key)
        {
            return key >= Key.D0 && key <= Key.D9 || key >= Key.NumPad0 && key <= Key.NumPad9;
        }

        public static bool IsIgnored(this Key key)
        {
            return key == Key.Up || key == Key.Down || key == Key.Tab || key == Key.Enter;
        }

    }
}
