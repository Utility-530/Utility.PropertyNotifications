using System;
using System.Collections.Generic;
using System.Text;
using static Utility.PatternMatchings.StringMatchers;

namespace Utility.PatternMatchings
{
    public static class TokenizedMatcher
    {

        public static bool TryMatch(PatternToken token, string pattern, out PatternMatchResult result)
        {
            result = default;
            if (pattern.Length == 0 || pattern.Length > token.Length)
                return false;

            foreach (var descriptor in MatchTable)
            {
                result = new PatternMatchResult(descriptor.Kind, 0, new List<TextSpan>());
                if (descriptor.Match(token, pattern, descriptor, result) is true)
                {
                    result.Score = AdjustScoreForPatternLength(result.Score * descriptor.PatternLengthMultiplier, pattern.Length);
                    return true;
                }
            }
            result = default;
            return false;
        }
        private static int AdjustScoreForPatternLength(int baseScore, int patternLength)
        {
            // Penalize very short patterns to reduce false positives
            return patternLength switch
            {
                <= 1 => baseScore + 50,
                2 => baseScore + 30,
                3 => baseScore + 10,
                _ => baseScore
            };
        }
        private struct MatchKindDescriptor
        {
            public PatternMatchKind Kind { get; }

            private Func<PatternToken, string, MatchKindDescriptor, PatternMatchResult, bool> matchFunc;

            public void SetSpan()
            {

            }

            public bool Match(PatternToken token, string pattern, MatchKindDescriptor descriptor, PatternMatchResult result)
            {
                return matchFunc(token, pattern, descriptor, result);
            }

            // Scoring parameters
            public int GapWeight { get; }                 // Penalty for skipped characters
            public int PositionWeight { get; }            // Weight for positions
            public int CaseMismatchPenalty { get; }       // Penalty if char case differs
            public int Bonus { get; }                     // Constant bonus
            public int PatternLengthMultiplier { get; }   // Multiplier for short patterns

            public MatchKindDescriptor(
                PatternMatchKind kind,
                Func<PatternToken, string, MatchKindDescriptor, PatternMatchResult, bool> matchFunc,
                int gapWeight = 0,
                int positionWeight = 1,
                int caseMismatchPenalty = 0,
                int bonus = 0,
                int patternLengthMultiplier = 1)
            {
                Kind = kind;
                this.matchFunc = matchFunc;
                GapWeight = gapWeight;
                PositionWeight = positionWeight;
                CaseMismatchPenalty = caseMismatchPenalty;
                Bonus = bonus;
                PatternLengthMultiplier = patternLengthMultiplier;
            }
        }

        private static readonly MatchKindDescriptor[] MatchTable =
{
    new MatchKindDescriptor(PatternMatchKind.Exact,
        (t, p, d, r) =>
        {
            r.Score = (int)d.Kind * 1000 + d.Bonus;
            if (p.Length != t.Length)
                return  false;
            for (int i = 0; i < p.Length; i++)
                if (t.Chars[i] != p[i])
                    return false;
            r.MatchedSpans.Add(new TextSpan(0, p.Length));
            return true;
        }),

    new MatchKindDescriptor(PatternMatchKind.Prefix,
        (t, p, d, r) =>
        {
            r.Score = (int)d.Kind * 1000 + d.Bonus;
            for (int i = 0; i < p.Length; i++)
                if (t.Chars[i] != p[i]) return false;
            r.MatchedSpans.Add(new TextSpan(0, p.Length));
            return true;
        }),

    new MatchKindDescriptor(PatternMatchKind.PrefixIgnoreCase,
        (t, p, d, r) =>
        {
            r.Score = (int)d.Kind * 1000 + d.Bonus;
            for (int i = 0; i < p.Length; i++)
            {
                if (char.ToLowerInvariant(t.Chars[i]) != char.ToLowerInvariant(p[i]))
                    return false;
                if (t.Chars[i] != p[i])
                    r.Score += d.CaseMismatchPenalty;
            }
            r.MatchedSpans.Add(new TextSpan(0, p.Length));
            return true;
        },
        caseMismatchPenalty: 1),

    new MatchKindDescriptor(PatternMatchKind.Acronym,
        (t, p, d, r) =>{
            bool match = AcronymMatch(t, p, r.MatchedSpans, out var score);
            r.Score = score;
            return match;
        },
        gapWeight: 1, positionWeight: 1, caseMismatchPenalty: 1),

    new MatchKindDescriptor(PatternMatchKind.CamelCase,
        (t, p, d, r) =>
        { var match = CamelCaseMatch(t, p, r.MatchedSpans, out var score);
            r.Score = score;
          return match;
        },
        gapWeight: 2, positionWeight: 1, caseMismatchPenalty: 1),

    new MatchKindDescriptor(PatternMatchKind.WordStart,
        (t, p, d, r) =>
        {
            if (!WordStartMatch(t, p, r.MatchedSpans, out var score))
                return false;
            score+=50;
            r.Score = score;
            return true;
        },
        gapWeight: 2, positionWeight: 1),

    new MatchKindDescriptor(PatternMatchKind.Substring,
        (t, p, d,r) => { var match = SubstringMatch(t, p, d.PositionWeight, r.MatchedSpans, out var score); r.Score = score + d.Bonus; return match; },
        positionWeight: 1),

    new MatchKindDescriptor(PatternMatchKind.Fuzzy,
        (t, p, d,r) => {var match = FuzzyMatch(t, p, r.MatchedSpans, out var score); r.Score = score; return match; },
        gapWeight: 5, positionWeight: 1, caseMismatchPenalty: 1)
        };
    }

}

