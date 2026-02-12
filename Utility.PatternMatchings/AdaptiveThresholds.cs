using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.PatternMatchings
{
    public static class AdaptiveThresholds
    {
        public static bool IsAllowed(PatternMatchKind kind, int patternLength)
        {
            return patternLength switch
            {
                <= 1 => kind <= PatternMatchKind.PrefixIgnoreCase,
                2 => kind <= PatternMatchKind.CamelCase,
                3 => kind <= PatternMatchKind.WordStart,
                4 => kind <= PatternMatchKind.Substring,
                _ => true
            };
        }
        public static int Difference(PatternMatchKind kind, int patternLength)
        {
            return patternLength switch
            {
                <= 1 => PatternMatchKind.Prefix - kind,
                2 => PatternMatchKind.CamelCase - kind,
                3 => PatternMatchKind.WordStart - kind,
                4 => PatternMatchKind.Substring - kind,
                _ => 1
            };
        }
    }

}
