using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public class Context : IContext
    {
        private readonly SynchronizationContext ui;

        public Context()
        {
            ui = SynchronizationContext.Current ?? throw new Exception($"Expected {nameof(SynchronizationContext)}!");
        }


        public SynchronizationContext UI => ui;

        public static Context Instance { get; } = new();

    }
}
