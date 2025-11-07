namespace Utility.Interfaces.Reactive.Generic
{
    public interface IObserver<T> : System.IObserver<T>
    {
        void OnProgress(int complete, int total);
    }
}