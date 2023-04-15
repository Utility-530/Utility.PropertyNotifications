using Utility.PropertyTrees.Abstractions;

namespace Utility.PropertyTrees.Abstractions
{
    public interface IControllable : IObservable<ControlType>
    {
        void Back();

        void Play();

        void Pause();

        void Forward();
    }
}