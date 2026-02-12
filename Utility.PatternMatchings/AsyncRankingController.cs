using System.Threading;

namespace Utility.PatternMatchings
{
    public sealed class AsyncRankingController
    {
        private CancellationTokenSource? _cts;

        public CancellationToken NewRequest()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            return _cts.Token;
        }
    }
}