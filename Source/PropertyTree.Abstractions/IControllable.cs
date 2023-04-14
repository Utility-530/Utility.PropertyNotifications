using Abstractions;

namespace PropertyTrees.WPF.Demo
{
    public interface IControllable : IObservable<ControlType>
    {
        void Back();

        void Play();

        void Pause();

        void Forward();
    }
}