using System.Runtime.CompilerServices;

namespace Utility.Models
{
    /// <summary>
    /// <a href="https://www.codeproject.com/Articles/5274659/How-to-Use-the-Csharp-Await-Keyword-on-Anything"></a>
    /// </summary>
    // modified from 
    // https://devblogs.microsoft.com/premier-developer/extending-the-async-methods-in-c/
    // premodifed source by Sergey Tepliakov
    public static class LazyUtility
    {
        // our awaiter type
        public readonly struct Awaiter<T> : INotifyCompletion
        {
            private readonly Lazy<T> _lazy;

            public Awaiter(Lazy<T> lazy) => _lazy = lazy;

            public T GetResult() => _lazy.Value;

            public bool IsCompleted => _lazy.IsValueCreated;

            public void OnCompleted(Action continuation)
            {
                // run the continuation if specified
                if (null != continuation)
                    Task.Run(continuation);
            }
        }
        // extension method for Lazy<T>
        // required for await support
        public static Awaiter<T> GetAwaiter<T>(this Lazy<T> lazy)
        {
            return new Awaiter<T>(lazy);
        }
    }
}


