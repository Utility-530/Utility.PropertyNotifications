using System.Reactive;
using Utility.Interfaces.Exs;
using Utility.Reactives;

namespace Utility.Models.Diagrams
{
    public class MethodConnection : IResolvableConnection
    {
        public static MethodConnection FromAction(Action<object> @in, IObservable<object> @out) => new (Observer.Create(@in), @out);

        public MethodConnection(IObserver<object> @in, IObservable<object> @out)
        {
            Disposable = @out.Subscribe(a =>
            {
                Transfer?.Invoke();
                @in.OnNext(a);
            });
            In = @in;
            Out = @out;
        }

        public event Action Transfer;
        public event Action TransferComplete;

        public void StopTransfer()
        {
            TransferComplete?.Invoke();
        }

        public IDisposable Disposable { get; }
        public IObserver<object> In { get; }
        public IObservable<object> Out { get; }
    }

}
