using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static WPF.ComboBoxes.Roslyn.RoslynPatternMatcher;

namespace WPF.ComboBoxes.Roslyn
{

    public sealed class IntelliSenseSession
    {
        private readonly IReadOnlyCollection<IndexedSymbol> _all;
        private IReadOnlyCollection<IndexedSymbol> _current;
        private string _lastPattern = "";
        private readonly MruTracker _mru;
        private readonly TelemetryTracker _telemetry;

        public IntelliSenseSession(IReadOnlyCollection<IndexedSymbol> symbols, MruTracker mru, TelemetryTracker telemetry)
        {
            _all = symbols;
            _current = symbols;
            _mru = mru;
            _telemetry = telemetry;
        }

        public IReadOnlyCollection<IndexedSymbol> CurrentSymbols => _current;

        // Build IntelliSenseResult from an IndexedSymbol + match
        public IntelliSenseResult BuildResult(IndexedSymbol symbol, PatternMatchResult match)
        {
            return new IntelliSenseResult(
                symbol,
                match,
                SymbolKindWeights.Get(symbol.Kind),
                NamespaceWeighting.GetPenalty(symbol.FullName),
                _mru.GetBoost(symbol.Item),
                _telemetry.GetBoost(symbol.Item)
            );
        }

        // Incremental narrowing + async-safe filter
        public List<IntelliSenseResult> Update(string pattern)
        {
            if (pattern.Length == 0)
            {
                _current = _all;
                _lastPattern = "";

                return _all
                    .Select(s => new IntelliSenseResult(
                        s,
                        default(PatternMatchResult),
                        SymbolKindWeights.Get(s.Kind),
                        NamespaceWeighting.GetPenalty(s.FullName),
                        _mru.GetBoost(s.Item),
                        _telemetry.GetBoost(s.Item)))
                    .OrderBy(r => r.Match.Score)
                    .ThenBy(r => r.Symbol.FullName)
                    .Take(200)
                    .ToList();
            }

            //if (string.IsNullOrEmpty(pattern))
            //{
            //    _current = _all;
            //    _lastPattern = "";

            //    return _all
            //        .Select(s => BuildResult(
            //            s,
            //            PatternMatchResult.Empty)) // special empty match
            //        .OrderBy(IntelliSenseResult.Compare)
            //        .Take(200) // important!
            //        .ToList();
            //}

            ReadOnlySpan<char> p = pattern.AsSpan();

            if (!pattern.StartsWith(_lastPattern, StringComparison.OrdinalIgnoreCase))
                _current = _all;

            var results = new List<IntelliSenseResult>(_current.Count);

            foreach (var s in _current)
            {
                if (TokenizedRoslynMatcher.TryMatch(s.Token, p, out var match))
                {
                    // Adaptive fuzzy threshold
                    if (!AdaptiveThresholds.IsAllowed(match.Kind, pattern.Length))
                        continue;

                    results.Add(BuildResult(s, match));
                }
            }

            results.Sort(IntelliSenseResult.Compare);
            _current = results.ConvertAll(r => r.Symbol).ToArray();
            _lastPattern = pattern;

            return results;
        }
    }

}
