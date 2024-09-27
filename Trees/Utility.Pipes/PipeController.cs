using System;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Windows.Input;
using Utility.Commands;
using Utility.ViewModels.Base;
using XPlat.UI;

namespace Utility.Pipes
{
    public class PipeController : BaseViewModel, IPipeLoader
    {
        private DispatcherTimer dispatcherTimer;
        private bool isPlaying;

        public PipeController()
        {


            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler<object>(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(0.1);
            dispatcherTimer.Start();


            StepCommand = new Command(() =>
            {
                isPlaying = false;
                Pipe.Instance.OnNext(Unit.Default);
            });

            //Pipe.Instance.Pending.CollectionChanged += Pending_CollectionChanged;
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            if (isPlaying)
                Pipe.Instance.OnNext(Unit.Default);
        }

        //private void Pending_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    OnPropertyChanged(nameof(Next));
        //}

        //public IEnumerable Pending => Pipe.Instance.Pending;
        //public IEnumerable Completed => Pipe.Instance.Completed;
        //public QueueItem Next => Pipe.Instance.Next;


        public void Initialise()
        {

        }

        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        public ICommand StepCommand { get; set; }
    }


    public interface IPipeLoader
    {
        void Initialise();
    }


}

