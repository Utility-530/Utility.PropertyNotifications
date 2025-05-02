using System.Collections.ObjectModel;
using System.Reactive;
using Utility.PropertyNotifications;

namespace Utility.Pipes
{
    public record Pipe : NotifyProperty, IObserver<Unit>
    {
        private SynchronizationContext current;
        private QueueItem selectedCompletedItem;

        public ObservableCollection<QueueItem> Forward { get; } = new();
        public ObservableCollection<QueueItem> Back { get; } = new();
        public ObservableCollection<QueueItem> Backlog { get; } = new();
        public ObservableCollection<QueueItem> Completed { get; } = new();

        public QueueItem SelectedCompletedItem
        {
            get => selectedCompletedItem; set
            {
                var previous = selectedCompletedItem;
                selectedCompletedItem = value;
                RaisePropertyReceived(value, previous);
            }
        }



        public QueueItem? Next => Forward.FirstOrDefault() ?? Back.FirstOrDefault() ?? Backlog.FirstOrDefault();

        Pipe()
        {
            current = SynchronizationContext.Current ?? throw new Exception("FGD $$$$$");
        }


        public void New(QueueItem forwardItem)
        {
            Backlog.Add(forwardItem);
        }

        public void GoBack(QueueItem queueItem)
        {

            Back.Add(queueItem);
        }


        public void Queue(QueueItem queueItem)
        {
            Forward.Add(queueItem);
        }

        public void OnNext(Unit unit)
        {
            current.Send((a) =>
            {

                if (Next != null)
                {
                    var next = Next;
                    Next.Invoke();

                    Completed.Add(next);
                    if (Backlog.Contains(next))
                        Backlog.Remove(next);
                    else if (Back.Contains(next))
                        Back.Remove(next);
                    else if (Forward.Contains(next))
                        Forward.Remove(next);

                }
            }, null);
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

