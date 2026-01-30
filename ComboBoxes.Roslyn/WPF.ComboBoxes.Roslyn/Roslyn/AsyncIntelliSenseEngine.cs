using System;
using System.Collections.Generic;
using System.Text;

namespace WPF.ComboBoxes.Roslyn
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using static WPF.ComboBoxes.Roslyn.RoslynPatternMatcher;

    public sealed class AsyncIntelliSenseEngine
    {
        private readonly IntelliSenseSession _session;
        private readonly AsyncRankingController _controller;

        public AsyncIntelliSenseEngine(IntelliSenseSession session, AsyncRankingController controller)
        {
            _session = session;
            _controller = controller;
        }

        // Main method to call on every keystroke
        public async Task<IReadOnlyList<IntelliSenseResult>> UpdateAsync(
            string pattern,
            Action<IReadOnlyList<IntelliSenseResult>> publishFast)
        {
            var token = _controller.NewRequest();

            // -------- PHASE 1: FAST (UI thread) --------
            var fastResults = new List<IntelliSenseResult>();
            foreach (var symbol in _session.CurrentSymbols)
            {
                if (TryFastMatch(symbol.Token, pattern.AsSpan(), out var match))
                {
                    fastResults.Add(_session.BuildResult(symbol, match));
                }
            }
            fastResults.Sort(IntelliSenseResult.Compare);
            publishFast(fastResults);

            // -------- PHASE 2: FULL (background) --------
            try
            {
                var fullResults = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    var results = _session.Update(pattern);
                    token.ThrowIfCancellationRequested();
                    return results;
                }, token);

                return fullResults;
            }
            catch (OperationCanceledException)
            {
                return fastResults;
            }
        }

        private static bool TryFastMatch(PatternToken token, ReadOnlySpan<char> pattern, out PatternMatchResult result)
        {
            if (TokenizedRoslynMatcher.TryMatch(token, pattern, out result))
            {
                // Only allow cheap matches for fast UI
                return result.Kind != PatternMatchKind.Fuzzy;
            }

            result = default;
            return false;
        }
    }

}
