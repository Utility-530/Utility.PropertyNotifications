using System;
using System.Collections.Generic;
using System.Text;
using Utility.PatternMatchings;

namespace Utility.PatternMatchings
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AsyncEngine
    {
        private readonly Session _session;
        private readonly AsyncRankingController _controller;

        public AsyncEngine(Session session, AsyncRankingController controller)
        {
            _session = session;
            _controller = controller;
        }

        // Main method to call on every keystroke
        public async Task<IReadOnlyList<Result>> UpdateAsync(
            string pattern,
            Action<IReadOnlyList<Result>> publishFast)
        {
            var token = _controller.NewRequest();

            // -------- PHASE 1: FAST (UI thread) --------
            var fastResults = new List<Result>();
            foreach (var symbol in _session.CurrentSymbols)
            {
                if (TryFastMatch(symbol.Token, pattern, out var match))
                {
                    fastResults.Add(_session.BuildResult(pattern, symbol, match));
                }
            }
            fastResults.Sort(Result.Compare);
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

        private static bool TryFastMatch(PatternToken token, string pattern, out PatternMatchResult result)
        {
            if (TokenizedMatcher.TryMatch(token, pattern, out result))
            {
                // Only allow cheap matches for fast UI
                return result.Kind != PatternMatchKind.Fuzzy;
            }

            result = default;
            return false;
        }
    }

}
