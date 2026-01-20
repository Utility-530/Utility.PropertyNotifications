using System;
using System.Collections.Generic;
using System.Text;

namespace WPF.ComboBoxes.Roslyn
{
    using System;
    using System.Collections.Generic;

    public enum PatternMatchKind
    {
        Acronym,
        Exact,
        Prefix,
        PrefixIgnoreCase,
        CamelCase,
        WordStart,
        Substring,
        Fuzzy
    }

    public readonly struct PatternMatchResult
    {
        public PatternMatchKind Kind { get; }
        public int Score { get; }
        public IReadOnlyList<TextSpan> MatchedSpans { get; }

        public PatternMatchResult(
            PatternMatchKind kind,
            int score,
            IReadOnlyList<TextSpan> spans)
        {
            Kind = kind;
            Score = score;
            MatchedSpans = spans;
        }
    }

    public readonly struct TextSpan
    {
        public int Start { get; }
        public int Length { get; }

        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }

    public static class RoslynPatternMatcher
    {
        public static class TokenizedRoslynMatcher
        {
            public static bool TryMatch(
                PatternToken token,
                ReadOnlySpan<char> pattern,
                out PatternMatchResult result)
            {
                result = default;

                if (pattern.Length == 0 || pattern.Length > token.Length)
                    return false;

                // 1. Exact
                if (ExactMatch(token, pattern))
                {
                    result = new PatternMatchResult(
                        PatternMatchKind.Exact,
                        0,
                        new[] { new TextSpan(0, pattern.Length) });
                    return true;
                }

                // 2. Prefix
                if (PrefixMatch(token, pattern, out int prefixScore))
                {
                    result = new PatternMatchResult(
                        PatternMatchKind.Prefix,
                        10 + prefixScore,
                        new[] { new TextSpan(0, pattern.Length) });
                    return true;
                }

                // 3. CamelCase / WordStart
                if (CamelCaseMatch(token, pattern, out var camelSpans, out int camelScore))
                {
                    result = new PatternMatchResult(
                        PatternMatchKind.CamelCase,
                        50 + camelScore,
                        camelSpans);
                    return true;
                }

                // 4. Acronym
                if (AcronymMatch(token, pattern, out var acronymSpans, out int acronymScore))
                {
                    result = new PatternMatchResult(
                        PatternMatchKind.Acronym,
                        80 + acronymScore,
                        acronymSpans);
                    return true;
                }

                // 5. Substring
                if (SubstringMatch(token, pattern, out int index))
                {
                    result = new PatternMatchResult(
                        PatternMatchKind.Substring,
                        200 + index * 3,
                        new[] { new TextSpan(index, pattern.Length) });
                    return true;
                }

                // 6. Fuzzy
                if (FuzzyMatch(token, pattern, out var fuzzySpans, out int fuzzyScore))
                {
                    result = new PatternMatchResult(
                        PatternMatchKind.Fuzzy,
                        500 + fuzzyScore,
                        fuzzySpans);
                    return true;
                }

                return false;
            }

            // ------------------------------------------------------------
            // Exact / Prefix
            // ------------------------------------------------------------

            private static bool ExactMatch(PatternToken t, ReadOnlySpan<char> p)
            {
                if (p.Length != t.Length)
                    return false;

                for (int i = 0; i < p.Length; i++)
                    if (t.Chars[i] != p[i])
                        return false;

                return true;
            }

            private static bool PrefixMatch(PatternToken t, ReadOnlySpan<char> p, out int score)
            {
                score = 0;

                for (int i = 0; i < p.Length; i++)
                {
                    if (t.LowerChars[i] != char.ToLowerInvariant(p[i]))
                        return false;

                    if (t.Chars[i] != p[i])
                        score++;
                }

                return true;
            }

            // ------------------------------------------------------------
            // CamelCase
            // ------------------------------------------------------------

            private static bool CamelCaseMatch(
                PatternToken t,
                ReadOnlySpan<char> p,
                out List<TextSpan> spans,
                out int score)
            {
                spans = new List<TextSpan>(p.Length);
                score = 0;
                int pi = 0;

                foreach (int wi in t.WordStarts)
                {
                    if (pi >= p.Length)
                        break;

                    if (char.ToLowerInvariant(t.Chars[wi]) ==
                        char.ToLowerInvariant(p[pi]))
                    {
                        spans.Add(new TextSpan(wi, 1));
                        score += wi;
                        if (t.Chars[wi] != p[pi])
                            score++;
                        pi++;
                    }
                }

                return pi == p.Length;
            }

            // ------------------------------------------------------------
            // Acronym
            // ------------------------------------------------------------

            private static bool AcronymMatch(
                PatternToken t,
                ReadOnlySpan<char> p,
                out List<TextSpan> spans,
                out int score)
            {
                spans = new List<TextSpan>(p.Length);
                score = 0;
                int pi = 0;

                foreach (int ui in t.UppercasePositions)
                {
                    if (pi >= p.Length)
                        break;

                    if (char.ToLowerInvariant(t.Chars[ui]) ==
                        char.ToLowerInvariant(p[pi]))
                    {
                        spans.Add(new TextSpan(ui, 1));
                        score += ui;
                        pi++;
                    }
                }

                return pi == p.Length;
            }

            // ------------------------------------------------------------
            // Substring
            // ------------------------------------------------------------

            private static bool SubstringMatch(
                PatternToken t,
                ReadOnlySpan<char> p,
                out int index)
            {
                for (int i = 0; i <= t.Length - p.Length; i++)
                {
                    bool match = true;
                    for (int j = 0; j < p.Length; j++)
                    {
                        if (t.LowerChars[i + j] != char.ToLowerInvariant(p[j]))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        index = i;
                        return true;
                    }
                }

                index = -1;
                return false;
            }

            // ------------------------------------------------------------
            // Fuzzy
            // ------------------------------------------------------------

            private static bool FuzzyMatch(
                PatternToken t,
                ReadOnlySpan<char> p,
                out List<TextSpan> spans,
                out int score)
            {
                spans = new List<TextSpan>(p.Length);
                score = 0;

                int pi = 0;
                int last = -1;

                for (int ti = 0; ti < t.Length && pi < p.Length; ti++)
                {
                    if (t.LowerChars[ti] == char.ToLowerInvariant(p[pi]))
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
}

