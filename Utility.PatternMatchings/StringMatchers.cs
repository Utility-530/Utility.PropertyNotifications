using System;
using System.Collections.Generic;
using System.Text;
using Utility.PatternMatchings;

namespace Utility.PatternMatchings
{
    public static class StringMatchers
    {
        public static bool MatchAtStarts(
            PatternToken t,
            ReadOnlySpan<char> p,
            IReadOnlyList<int> starts,
            MatchMode mode,
            PatternMatchKind kind,           
            out int score, 
            IList<TextSpan> spans = null)
        {
            spans ??= new List<TextSpan>(p.Length);
            score = (int)kind * 1000; // monotonic base
            int pi = 0;
            int last = -1;

            foreach (int si in starts)
            {
                if (pi >= p.Length) break;

                char tokenChar = t.Chars[si];
                char patternChar = p[pi];

                if (mode == MatchMode.Acronym && !char.IsUpper(tokenChar))
                    continue;

                if (char.ToLowerInvariant(tokenChar) == char.ToLowerInvariant(patternChar))
                {
                    spans.Add(new TextSpan(si, 1));

                    // earlier positions are better
                    score += si;

                    // penalize gaps
                    if (last >= 0)
                        score += (si - last - 1) * 2;

                    // case mismatch penalty (optional)
                    if (mode == MatchMode.Acronym && tokenChar != patternChar)
                        score++;

                    last = si;
                    pi++;
                }
            }

            return pi == p.Length;
        }
        
        public static bool AcronymMatch(PatternToken t, ReadOnlySpan<char> p, IList<TextSpan> spans, out int score)
        {        
            return MatchAtStarts(t, p, t.UppercasePositions, MatchMode.Acronym, PatternMatchKind.Acronym, out score, spans);
        }

        public static bool CamelCaseMatch(PatternToken t, ReadOnlySpan<char> p, IList<TextSpan> spans, out int score)
        {
            return MatchAtStarts(t, p, t.UppercasePositions, MatchMode.Default, PatternMatchKind.CamelCase, out score, spans);
        }

        public static bool WordStartMatch(PatternToken t, ReadOnlySpan<char> p, IList<TextSpan> spans, out int score)
        {
            if (!MatchAtStarts(t, p, t.WordStarts, MatchMode.Default, PatternMatchKind.WordStart, out score, spans))
                return false;

            score += 50; // slightly weaker than CamelCase
            return true;
        }

        //public static bool ExactMatch(PatternToken t, ReadOnlySpan<char> p)
        //{
        //    if (p.Length != t.Length) return false;
        //    for (int i = 0; i < p.Length; i++)
        //        if (t.Chars[i] != p[i]) return false;
        //    return true;
        //}

        //public static bool PrefixMatch(PatternToken t, ReadOnlySpan<char> p)
        //{
        //    for (int i = 0; i < p.Length; i++)
        //        if (t.Chars[i] != p[i]) return false;
        //    return true;
        //}

        //public static bool PrefixIgnoreCaseMatch(PatternToken t, ReadOnlySpan<char> p, out int score)
        //{
        //    score = (int)PatternMatchKind.PrefixIgnoreCase * 1000;
        //    for (int i = 0; i < p.Length; i++)
        //    {
        //        if (char.ToLowerInvariant(t.Chars[i]) != char.ToLowerInvariant(p[i]))
        //            return false;
        //        if (t.Chars[i] != p[i])
        //            score++;
        //    }
        //    return true;
        //}
        public static bool SubstringMatch(
            PatternToken t,
            ReadOnlySpan<char> p,
            int positionWeight,
            IList<TextSpan> spans,
            out int score)
        {
            score = (int)PatternMatchKind.Substring * 1000;

            for (int i = 0; i <= t.Length - p.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < p.Length; j++)
                {
                    if (char.ToLowerInvariant(t.Chars[i + j]) != char.ToLowerInvariant(p[j]))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    spans.Add(new TextSpan(i, p.Length));
                    score += i * positionWeight;
                    return true;
                }
            }

            return false;
        }

        public static bool FuzzyMatch(PatternToken t, ReadOnlySpan<char> p, IList<TextSpan> spans, out int score)
        {
            score = (int)PatternMatchKind.Fuzzy * 1000;

            int pi = 0;
            int last = -1;

            for (int ti = 0; ti < t.Length && pi < p.Length; ti++)
            {
                if (char.ToLowerInvariant(t.Chars[ti]) == char.ToLowerInvariant(p[pi]))
                {
                    spans.Add(new TextSpan(ti, 1));

                    if (last >= 0)
                        score += (ti - last - 1) * 5;

                    score += ti;
                    if (t.Chars[ti] != p[pi])
                        score++;

                    last = ti;
                    pi++;
                }
            }

            return pi == p.Length;
        }
    }
}
