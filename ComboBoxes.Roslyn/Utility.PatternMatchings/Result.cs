using System;
using System.Collections.Generic;
using System.Text;
using Utility.PatternMatchings;

namespace Utility.PatternMatchings
{
    public sealed class Input
    {
        public readonly object Item;
        public readonly PatternToken Token;
        public readonly int Kind;
        public readonly string FullName;

        public Input(object item, PatternToken token,
                             int kind, string fullName)
        {
            Item = item;
            Token = token;
            Kind = kind;
            FullName = fullName;
        }
    }


    public sealed class Result
    {
        public readonly Input Symbol;
        public readonly PatternMatchResult Match;
        private readonly int _symbolWeight;
        private readonly int _namespacePenalty;
        private readonly int _mruBoost;
        private readonly int _telemetryBoost;
        private bool isChecked;

        public Result(
            string pattern,
            Input symbol,
            PatternMatchResult match,
           
            int symbolWeight,
            int namespacePenalty,
            int mruBoost,
            int telemetryBoost)
        {
            Pattern = pattern;
            Symbol = symbol;
            Match = match;
            _symbolWeight = symbolWeight;
            _namespacePenalty = namespacePenalty;
            _mruBoost = mruBoost;
            _telemetryBoost = telemetryBoost;
        }

        public bool IsAllowed => AdaptiveThresholds.IsAllowed(Match.Kind, Pattern.Length);
        public bool IsChecked { get => isChecked; set => isChecked = value; }

        public string Pattern { get; }

        // VS-style sorting
        public static int Compare(Result a, Result b)
        {
            int c = a.Match.Score.CompareTo(b.Match.Score);
            if (c != 0) return c;

            c = a.Match.Kind.CompareTo(b.Match.Kind);
            if (c != 0) return c;

            c = a._symbolWeight.CompareTo(b._symbolWeight);
            if (c != 0) return c;

            c = a._namespacePenalty.CompareTo(b._namespacePenalty);
            if (c != 0) return c;

            c = a._mruBoost.CompareTo(b._mruBoost);
            if (c != 0) return c;

            c = a._telemetryBoost.CompareTo(b._telemetryBoost);
            if (c != 0) return c;

            return string.Compare(a.Symbol.Token.Text,
                                  b.Symbol.Token.Text,
                                  StringComparison.OrdinalIgnoreCase);
        }
    }

}
