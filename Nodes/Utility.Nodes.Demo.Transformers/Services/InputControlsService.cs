using System.Reactive.Linq;
using Utility.Nodes.Meta;
using Utility.Models;
using Utility.Interfaces.Exs;
using Splat;
using System.Reactive.Subjects;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Interfaces.Generic;
using Utility.Nodes.Demo.Transformers.Services;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using System.ComponentModel;

namespace Utility.Nodes.Demo.Filters.Services
{
    public class InputControlsService : Model, IObservable<ControlEvent>
    {
        Dictionary<Guid, IDisposable> disposables = new();
        ReplaySubject<ControlEvent> replaySubject = new(1);
        Dictionary<ControlEventType, int> dict = new();


        public InputControlsService()
        {
            Locator.Current.GetService<IObservableIndex<INodeViewModel>>()
                [nameof(NodeMethodFactory.BuildInputControlRoot)]
                .Subscribe(_n =>
                {
                    Utility.Trees.Extensions.Async.Match.Descendants(_n)
                    .Subscribe(node =>
                    {
                        if (node.NewItem is IGetName { Name: string name } and INotifyPropertyChanged  model )
                        {
                            model.WhenChanged().WhereIsNull().Subscribe(_ =>
                            {
                                Switch(name, model);
                            });
                        }
                    });
                });
        }

        private void Switch(string name, INotifyPropertyChanged model)
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
