using Utility.Interfaces.Exs;

namespace Utility.Models.Diagrams
{
    public class MethodConnection : IResolvableConnection
    {
        public MethodConnection(Action<object> @in, IObservable<object> @out)
        {
            Disposable = @out.Subscribe(a =>
            {
                Transfer?.Invoke();
                @in.Invoke(a);
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
        public Action<object> In { get; }
        public IObservable<object> Out { get; }
    }

}
