using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.WPF.ComboBoxes.Roslyn
{
    using System;

    public sealed class PatternToken
    {
        public readonly string Text;
        public readonly char[] Chars;
        public readonly char[] LowerChars;
        public readonly int Length;
        public readonly int[] WordStarts;
        public readonly int[] UppercasePositions;

        public PatternToken(string text)
        {
            Text = text;
            Chars = text.ToCharArray();
            Length = Chars.Length;

            LowerChars = new char[Length];
            for (int i = 0; i < Length; i++)
                LowerChars[i] = char.ToLowerInvariant(Chars[i]);

            WordStarts = ComputeWordStarts();
            UppercasePositions = ComputeUppercasePositions();
        }

        private int[] ComputeWordStarts()
        {
            int[] temp = new int[Length];
            int count = 0;

            for (int i = 0; i < Length; i++)
            {
                if (i == 0 ||
                    !char.IsLetterOrDigit(Chars[i - 1]) ||
                    (char.IsLower(Chars[i - 1]) && char.IsUpper(Chars[i])))
                {
                    temp[count++] = i;
                }
            }

            return Trim(temp, count);
        }

        private int[] ComputeUppercasePositions()
        {
            int[] temp = new int[Length];
            int count = 0;

            for (int i = 0; i < Length; i++)
            {
                if (char.IsUpper(Chars[i]))
                    temp[count++] = i;
            }

            return Trim(temp, count);
        }

        private static int[] Trim(int[] buffer, int count)
        {
            var result = new int[count];
            Array.Copy(buffer, result, count);
            return result;
        }
    }

}
