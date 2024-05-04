namespace Utility.Nodes.Demo
{
    public class EventListener : IEventListener, IObservable<object>
    {
        ReplaySubject<object> replay = new();
        Stack<TreeViewItem> stack = new();
        private EventListener()
        {
        }

        public void Send(IEvent @event)
        {
            if (@event is OnHoverChange { Source: TreeViewItem treeViewItem } hoverChange)
            {
                AdornerController.Instance.OnNext(hoverChange);
                return;
            }
            if (@event is DoubleClickChange clickChange)
            {
                return;
            }
            if (@event is ClickChange { Node: IReadOnlyTree{ Data: ICollectionItemDescriptor { } descriptor } node }  _clickChange)
            {
                replay.OnNext(descriptor);
                return;
            }
            if (@event is OnLoadedChange { Source: TreeViewItem _treeViewItem } loadedChange)
            {
                AdornerController.Instance.OnNext(loadedChange);
                return;
            }
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return replay.Subscribe(observer);
        }

        public static EventListener Instance { get; } = new();


    }
}
