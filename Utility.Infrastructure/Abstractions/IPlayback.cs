using Utility.Interfaces.NonGeneric;


namespace Utility.PropertyTrees.Abstractions
{
    public interface IPlayback : IObservable, IObserver
    {
        void Back();

        void Play();

        void Pause();

        void Forward();
    }
}