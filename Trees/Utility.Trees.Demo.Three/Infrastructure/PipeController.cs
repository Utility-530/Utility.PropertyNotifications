
//using Jellyfish;
//using Splat;
//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Reactive;
//using System.Threading;
//using System.Windows.Input;
//using System.Windows.Threading;
//using Utility.Trees.Decisions;
//using Utility.Trees.Demo.MVVM.Infrastructure;
//using Utility.ViewModels.Base;

using Splat;
using System;
using Utility.Pipes;
using Utility.Trees.Decisions;
using Utility.Trees.Demo.MVVM.Infrastructure;

namespace Utility.Trees.Demo.MVVM
{
    //    public class PipeController : ViewModel, IPipeLoader
    //    {
    //        private DispatcherTimer dispatcherTimer;
    //        private bool isPlaying;

    //        public PipeController()
    //        {


    //            dispatcherTimer = new DispatcherTimer();
    //            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
    //            dispatcherTimer.Interval = TimeSpan.FromSeconds(0.1);
    //            dispatcherTimer.Start();


    //            StepCommand = new RelayCommand((a) =>
    //            {
    //                isPlaying = false;
    //                Pipe.Instance.OnNext(Unit.Default);
    //            });

    //            Pipe.Instance.Pending.CollectionChanged += Pending_CollectionChanged;
    //        }

    //        private void dispatcherTimer_Tick(object sender, EventArgs e)
    //        {
    //            if (isPlaying)
    //                Pipe.Instance.OnNext(Unit.Default);
    //        }

    //        private void Pending_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    //        {
    //            OnPropertyChanged(nameof(Next));
    //        }

    //        //public IEnumerable Pending => Pipe.Instance.Pending;
    //        //public IEnumerable Completed => Pipe.Instance.Completed;
    //        public QueueItem Next => Pipe.Instance.Next;


    //        public void Initialise()
    //        {

    //        }

    //        public bool IsPlaying
    //        {
    //            get => isPlaying;
    //            set
    //            {
    //                isPlaying = value;
    //                OnPropertyChanged(nameof(IsPlaying));
    //            }
    //        }

    //        public ICommand StepCommand { get; set; }
    //    }


    //    public interface IPipeLoader
    //    {
    //        void Initialise();
    //    }

    //    public class PipeLoader : IPipeLoader
    //    {
    //        Timer timer;

    //        public PipeLoader()
    //        {
    //        }

    //        public void Initialise()
    //        {
    //            timer = new Timer(Timer_Tick, default, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.01));
    //        }

    //        private void Timer_Tick(object sender)
    //        {
    //            Pipe.Instance.OnNext(Unit.Default);
    //        }
    //    }


    //    public class Pipe : BaseViewModel, IObserver<Unit>
    //    {
    //        public ObservableCollection<QueueItem> Pending { get; } = new();
    //        public ObservableCollection<QueueItem> Completed { get; } = new();
    //        public QueueItem Last { get; set; }
    //        public QueueItem? Next => Pending.FirstOrDefault();

    //        Pipe()
    //        {
    //        }

    //        public void Queue(QueueItem queueItem)
    //        {
    //            if (queueItem is RepoQueueItem { QueueItemType: QueueItemType.Find, Guid: { } guid })
    //            {
    //                if (guid == Guid.Empty)
    //                {

    //                }
    //            }

    //            Pending.Add(queueItem);

    //            if (Next == null && Pending.Any() == false)
    //            {
    //                //this.OnPropertyChanged(nameof(Next));
    //            }
    //        }

    //        public void OnNext(Unit unit)
    //        {
    //            //lock (Pending)
    //            //    if (Pending.Any() == false)
    //            //    {
    //            //    }
    //            //    else
    //            //    {
    //            //        //Next = (QueueItem?)Pending.OfType<DecisionTreeQueueItem>().FirstOrDefault() ?? Pending.OfType<RepoQueueItem>().FirstOrDefault();
    //            //        OnPropertyChanged(nameof(Next));
    //            //    }

    //            if (Next != null)
    //            {
    //                //if (Pending.Contains(Next))


    //                //Last = Next;
    //                //OnPropertyChanged(nameof(Last));
    //                if (Next is RepoQueueItem repoQueueItem)
    //                {
    //                    Splat.Locator.Current.GetService<PipeRepository>().OnNext(repoQueueItem);
    //                }
    //                //else if (Last is DecisionTreeQueueItem decisionTreeQueueItem)
    //                //    decisionTreeQueueItem.DecisionTree.Eval();

    //                Completed.Add(Next);
    //                Pending.Remove(Next);
    //                //Next = null;
    //            }
    //        }

    //        public void OnError(Exception error)
    //        {
    //            throw new NotImplementedException();
    //        }

    //        public void OnCompleted()
    //        {
    //            throw new NotImplementedException();
    //        }

    //        public static Pipe Instance { get; } = new();
    //    }


    //    public record QueueItem
    //    {
    //        public QueueItem()
    //        {
    //            DateTime = DateTime.Now;
    //        }

    //        public DateTime DateTime { get; }
    //    }

    public record RepoQueueItem(Guid Guid, QueueItemType QueueItemType, string? Name = default, Type? Type = default, int? Index = default, string? TableName = default, Guid? ParentGuid = default) : QueueItem
    {
        public virtual bool Equals(RepoQueueItem? other)
        {
            if ((other != null))
            {
                return this.Guid == other.Guid &&
                    this.QueueItemType == other.QueueItemType &&
                    this.Name == other.Name &&
                    this.Type == other.Type &&
                    this.Index == other.Index &&
                    this.TableName == other.TableName &&
                    this.ParentGuid == other.ParentGuid;

            }
            return false;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override void Invoke()
        {
            Splat.Locator.Current.GetService<PipeRepository>().OnNext(this);
        }
    }

    //public record DecisionTreeQueueItem(DecisionTree DecisionTree) : QueueItem;

    public enum QueueItemType
    {
        Get, Find, SelectKeys
    }
}

