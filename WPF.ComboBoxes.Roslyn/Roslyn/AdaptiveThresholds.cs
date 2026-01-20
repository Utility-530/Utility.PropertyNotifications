using System;
using System.Collections.Generic;
using System.Text;

namespace WPF.ComboBoxes.Roslyn
{
    public static class AdaptiveThresholds
    {
        public static bool IsAllowed(PatternMatchKind kind, int patternLength)
        {
            return patternLength switch
            {
                <= 1 => kind <= PatternMatchKind.Prefix,
                2 => kind <= PatternMatchKind.CamelCase,
                3 => kind <= PatternMatchKind.WordStart,
                4 => kind <= PatternMatchKind.Substring,
                _ => true
            };
        }
    }

}
