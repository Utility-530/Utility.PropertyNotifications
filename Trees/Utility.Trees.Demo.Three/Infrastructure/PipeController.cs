
using Jellyfish;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
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
        public ObservableCollection<QueueItem> Pending { get; } = new();
        public ObservableCollection<QueueItem> Completed { get; } = new();
        public QueueItem Last { get; set; }
        public QueueItem Next { get; set; }

        Pipe()
        {
        }

        public void Queue(QueueItem queueItem)
        {
            if (Next == null && Pending.Any()==false)
            {
                Next = queueItem;
                this.OnPropertyChanged(nameof(Next));

            }
            else
                Pending.Add(queueItem);
        }

        public void OnNext(Unit unit)
        {
            lock (Pending)
                if (Pending.Any() == false)
                {
                }
                else
                {
                    Next = (QueueItem?)Pending.OfType<DecisionTreeQueueItem>().FirstOrDefault() ?? Pending.OfType<RepoQueueItem>().FirstOrDefault();
                    OnPropertyChanged(nameof(Next));
                }

            if (Next != null)
            {
                if (Pending.Contains(Next))
                    Pending.Remove(Next);

                Last = Next;
                OnPropertyChanged(nameof(Last));
                if (Last is RepoQueueItem repoQueueItem)
                {
                    Splat.Locator.Current.GetService<PipeRepository>().OnNext(repoQueueItem);
                }
                else if (Last is DecisionTreeQueueItem decisionTreeQueueItem)
                    decisionTreeQueueItem.DecisionTree.Eval();

                Completed.Add(Last);
                Next = null;
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


    public record QueueItem()
    {
    }

    public record RepoQueueItem(Guid Guid, QueueItemType QueueItemType, string? Name = default, Type? Type = default, int? Index = default, string? TableName = default, Guid? ParentGuid = default) : QueueItem
    {
    }

    public record DecisionTreeQueueItem(DecisionTree DecisionTree) : QueueItem;

    public enum QueueItemType
    {
        Get, Find, SelectKeys
    }
}

