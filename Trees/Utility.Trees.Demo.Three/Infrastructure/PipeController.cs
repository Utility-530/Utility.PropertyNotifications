
using Jellyfish;
using Splat;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Repos;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.ViewModels.Base;

namespace Utility.Trees.Demo.MVVM
{
    public class PipeController : ViewModel, IPipeLoader
    {
        private DispatcherTimer dispatcherTimer;
        private bool isPlaying;

        public PipeController()
        {


            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(0.1);
            dispatcherTimer.Start();


            StepCommand = new RelayCommand((a) =>
            {
                isPlaying = false;
                Pipe.Instance.OnNext(Unit.Default);
            });

            Pipe.Instance.Pending.CollectionChanged += Pending_CollectionChanged;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (isPlaying)
                Pipe.Instance.OnNext(Unit.Default);
        }

        private void Pending_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Next));
        }

        //public IEnumerable Pending => Pipe.Instance.Pending;
        //public IEnumerable Completed => Pipe.Instance.Completed;
        public QueueItem Next => Pipe.Instance.Next;


        public void Initialise()
        {

        }

        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }
        public bool IsPaused
        {
            get => isPlaying == false;
            set
            {
                isPlaying = !value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
        public ICommand StepCommand { get; set; }
    }


    public interface IPipeLoader
    {
        void Initialise();
    }

    public class PipeLoader : IPipeLoader
    {
        Timer timer;

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


    public class Pipe : BaseViewModel, IObserver<Unit>
    {
        private readonly TaskFactory factory;
        //private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Timer timer;

        public ObservableCollection<QueueItem> Pending { get; } = new();
        public ObservableCollection<QueueItem> Completed { get; } = new();
        public QueueItem Last { get; set; }
        public QueueItem Next
        {
            get
            {
                var item = Pending.FirstOrDefault();
                return item;
            }
        }

        Pipe()
        {

            factory = new TaskFactory(new ConstrainedTaskScheduler() { MaximumTaskCount = 1 });
           
        }


        public void Queue(QueueItem queueItem)
        {
            Pending.Add(queueItem);  
        }

        public void OnNext(Unit unit)
        {
            lock (Pending)
                if (Pending.ToArray().Any())
                //if (Pending.TryDequeue(out var item))
                {
                    Last = Next;
                    this.OnPropertyChanged(nameof(Next));
                    Pending.RemoveAt(0);
                    
                    factory.StartNew(() => Splat.Locator.Current.GetService<PipeRepository>().Select(Last.Guid))
                        .ContinueWith(tsk =>
                        {
                            Completed.Add(Last);
                        }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext())
                        .ContinueWith(tsk =>
                        {
                            var flattenedException = tsk.Exception.Flatten();
                            //AddLog("Exception! " + flattenedException);
                            return true;
                        }, TaskContinuationOptions.OnlyOnFaulted);

                }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public static Pipe Instance { get; } = new();
    }

    public record QueueItem(Guid Guid);
}

