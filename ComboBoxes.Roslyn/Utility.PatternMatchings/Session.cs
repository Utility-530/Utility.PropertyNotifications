using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Utility.PatternMatchings;

namespace Utility.PatternMatchings
{

    public sealed class Session
    {
        private readonly IReadOnlyCollection<Input> _all;
        private IReadOnlyCollection<Input> _current;
        private string _lastPattern = "";
        private readonly MruTracker _mru;
        private readonly TelemetryTracker _telemetry;
        private readonly Func<int, int> weigher;

        public Session(IReadOnlyCollection<Input> symbols, MruTracker mru, TelemetryTracker telemetry, Func<int, int> weigher)
        {
            _all = symbols;
            _current = symbols;
            _mru = mru;
            _telemetry = telemetry;
            this.weigher = weigher;
        }

        public IReadOnlyCollection<Input> CurrentSymbols => _current;

        // Build IntelliSenseResult from an IndexedSymbol + match
        public Result BuildResult(string pattern, Input symbol, PatternMatchResult match)
        {
            return new Result(
                pattern,
                symbol,
                match,
                weigher(symbol.Kind),
                NamespaceWeighting.GetPenalty(symbol.FullName),
                _mru.GetBoost(symbol.Item),
                _telemetry.GetBoost(symbol.Item)
            );
        }

        // Incremental narrowing + async-safe filter
        public List<Result> Update(string pattern, int? count = null)
        {
            if (pattern.Length == 0)
            {
                _current = _all;
                _lastPattern = "";

                return _all
                    .Select(s => new Result(
                        pattern,
                        s,
                        default(PatternMatchResult),
                        weigher(s.Kind),
                        NamespaceWeighting.GetPenalty(s.FullName),
                        _mru.GetBoost(s.Item),
                        _telemetry.GetBoost(s.Item)))
                    .OrderBy(r => r.Match?.Score ?? 0)
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

            string p = pattern;

            if (!pattern.StartsWith(_lastPattern, StringComparison.OrdinalIgnoreCase) || _current.Any() == false)
                _current = _all;

            var results = new List<Result>(_current.Count);

            foreach (var s in _current)
            {
                if (TokenizedMatcher.TryMatch(s.Token, p, out var match))
                {
                    // Adaptive fuzzy threshold
                    if (AdaptiveThresholds.IsAllowed(match.Kind, pattern.Length))
                        results.Add(BuildResult(pattern, s, match));
                }
            }

            //if (count != null)
            //{
            //    results = results.Where(a => a.IsAllowed).Take(count.Value).ToList();
            //}

            results.Sort(Result.Compare);
            _current = results.ConvertAll(r => r.Symbol).ToArray();
            _lastPattern = pattern;

            return results;
        }
    }

}
