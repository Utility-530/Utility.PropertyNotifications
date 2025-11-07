using System.Reactive;

namespace Utility.Pipes
{
    public class PipeLoader : IPipeInitialiser
    {
        private Timer timer;

        public PipeLoader()
        {
        }

        public void Initialise()
        {
            timer = new Timer(Timer_Tick, default, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.01));
        }

        private void Timer_Tick(object sender)
        {
            Pipe.Instance.OnNext(Unit.Default);
        }
    }
}