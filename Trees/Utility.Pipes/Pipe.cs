using System.Collections.ObjectModel;
using System.Reactive;
using Utility.ViewModels.Base;

namespace Utility.Pipes
{
    public class Pipe : BaseViewModel, IObserver<Unit>
    {
        private SynchronizationContext current;

        public ObservableCollection<QueueItem> Pending { get; } = new();
        public ObservableCollection<QueueItem> Completed { get; } = new();
        public QueueItem Last { get; set; }
        public QueueItem? Next => Pending.FirstOrDefault();

        Pipe()
        {
            current = SynchronizationContext.Current?? throw new Exception("FGD $$$$$"); 
        }

        public void Queue(QueueItem queueItem)
        {
            //if (queueItem is RepoQueueItem { QueueItemType: QueueItemType.Find, Guid: { } guid })
            //{
            //    if (guid == Guid.Empty)
            //    {

            //    }
            //}

            Pending.Add(queueItem);

            if (Next == null && Pending.Any() == false)
            {
                //this.OnPropertyChanged(nameof(Next));
            }
        }

        public void OnNext(Unit unit)
        {
            //lock (Pending)
            //    if (Pending.Any() == false)
            //    {
            //    }
            //    else
            //    {
            //        //Next = (QueueItem?)Pending.OfType<DecisionTreeQueueItem>().FirstOrDefault() ?? Pending.OfType<RepoQueueItem>().FirstOrDefault();
            //        OnPropertyChanged(nameof(Next));
            //    }

            if (Next != null)
            {
                //if (Pending.Contains(Next))


                //Last = Next;
                //OnPropertyChanged(nameof(Last));
                //if (Next is RepoQueueItem repoQueueItem)
                //{
                //    Splat.Locator.Current.GetService<PipeRepository>().OnNext(repoQueueItem);
                //}

                current.Send((a) =>
                {
                    Next.Invoke();
                    //else if (Last is DecisionTreeQueueItem decisionTreeQueueItem)
                    //   decisionTreeQueueItem.DecisionTree.Eval();

                    Completed.Add(Next);
                    Pending.Remove(Next);
                }, null);
                //Next = null;
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
}

