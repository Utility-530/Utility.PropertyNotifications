using System;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Timers;
using System.Windows.Input;
using Utility.Commands;
using Utility.PropertyNotifications;
using XPlat.UI;

namespace Utility.Pipes
{
    public class PipeController : NotifyPropertyClass, IPipeInitialiser
    {
        private DispatcherTimer dispatcherTimer;
        private System.Threading.Timer timer;
        private bool isPlaying;

        public PipeController()
        {


            dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler<object>(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(0.1);
            dispatcherTimer.Start();

            timer = new System.Threading.Timer(callback, null, 0, 10);

            StepCommand = new Command(() =>
            {
                isPlaying = false;
                Pipe.Instance.OnNext(Unit.Default);
            });

            //Pipe.Instance.Pending.CollectionChanged += Pending_CollectionChanged;
        }

        private void callback(object? state)
        {
            if (isPlaying)
                Pipe.Instance.OnNext(Unit.Default);
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            if (isPlaying)
                Pipe.Instance.OnNext(Unit.Default);
        }

        public void Initialise()
        {

        }

        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                RaisePropertyChanged(nameof(IsPlaying));
            }
        }

        public ICommand StepCommand { get; set; }
    }


    public interface IPipeInitialiser
    {
        void Initialise();
    }


}

