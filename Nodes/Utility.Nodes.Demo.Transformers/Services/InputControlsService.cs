using System.Reactive.Linq;
using Utility.Nodes.Filters;
using Utility.Models;
using Utility.Interfaces.Exs;
using Splat;
using System.Reactive.Subjects;
using Utility.PropertyNotifications;
using Utility.Models.Trees;
using Utility.Reactives;

namespace Utility.Nodes.Demo.Filters.Services
{
    public class InputControlsService : Model, IObservable<ControlEvent>
    {
        Dictionary<Guid, IDisposable> disposables = new();
        ReplaySubject<ControlEvent> replaySubject = new(1);
        Dictionary<ControlEventType, int> dict = new();


        public InputControlsService()
        {
            Locator.Current.GetService<INodeSource>()
                .Single(nameof(NodeMethodFactory.BuildInputControlRoot))
                .Subscribe(_n =>
                {
                    Utility.Trees.Extensions.Async.Match.Descendants(_n)
                    .Subscribe(node =>
                    {
                        if (node.NewItem is INode { Data: Model { Name: string name } model })
                        {
                            model.WhenChanged().WhereIsNull().Subscribe(_ =>
                            {
                                Switch(name, model);
                            });
                        }
                    });
                });
        }

        private void Switch(string name, Model model)
        {
            switch (name)
            {
                case NodeMethodFactory.Select:
                    {
                        replaySubject.OnNext(new ControlEvent(ControlEventType.Select, default));
                    }
                    break;
                case NodeMethodFactory.Cancel:
                    {
                        replaySubject.OnNext(new ControlEvent(ControlEventType.Cancel, default));
                    }
                    break;
            }
        }

        public IDisposable Subscribe(IObserver<ControlEvent> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }
}
