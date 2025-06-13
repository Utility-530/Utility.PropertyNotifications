using System;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;

namespace Utility.Models
{
    public record GuidBase(Guid Guid) : IGetGuid
    {
        public GuidBase() : this(Guid.NewGuid())
        {
        }

        private GuidBase(Guid Guid, Progress progress) : this(Guid)
        {
            this.Progress = progress;
        }

        private GuidBase(Guid Guid, Exception Exception) : this(Guid)
        {
            this.Exception = Exception;
        }
        private GuidBase(Guid Guid, bool IsComplete) : this(Guid)
        {
            this.IsComplete = IsComplete;
        }

        public Exception? Exception { get; }

        public bool IsComplete { get; }

        public Progress Progress { get; }

        public static GuidBase OnCompleted(Guid guid)
        {
            return new GuidBase(guid, true);
        }
        public static GuidBase OnError(Guid guid, Exception exception)
        {
            return new GuidBase(guid, exception);
        }

        public static GuidBase OnProgress(Guid guid, int a, int b)
        {
            return new GuidBase(guid, new Progress(a, b));
        }
    }
}
