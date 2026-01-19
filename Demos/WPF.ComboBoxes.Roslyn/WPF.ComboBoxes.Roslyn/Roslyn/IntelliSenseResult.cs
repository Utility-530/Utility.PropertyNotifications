using System;
using System.Collections.Generic;
using System.Text;

namespace WPF.ComboBoxes.Roslyn
{
    public sealed class IndexedSymbol
    {
        public readonly object Item;
        public readonly PatternToken Token;
        public readonly IntelliSenseSymbolKind Kind;
        public readonly string FullName;

        public IndexedSymbol(object item, PatternToken token,
                             IntelliSenseSymbolKind kind, string fullName)
        {
            Item = item;
            Token = token;
            Kind = kind;
            FullName = fullName;
        }
    }


    public sealed class IntelliSenseResult
    {
        public readonly IndexedSymbol Symbol;
        public readonly PatternMatchResult Match;
        private readonly int _symbolWeight;
        private readonly int _namespacePenalty;
        private readonly int _mruBoost;
        private readonly int _telemetryBoost;

        public IntelliSenseResult(
            IndexedSymbol symbol,
            PatternMatchResult match,
            int symbolWeight,
            int namespacePenalty,
            int mruBoost,
            int telemetryBoost)
        {
            Symbol = symbol;
            Match = match;
            _symbolWeight = symbolWeight;
            _namespacePenalty = namespacePenalty;
            _mruBoost = mruBoost;
            _telemetryBoost = telemetryBoost;
        }

        // VS-style sorting
        public static int Compare(IntelliSenseResult a, IntelliSenseResult b)
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
