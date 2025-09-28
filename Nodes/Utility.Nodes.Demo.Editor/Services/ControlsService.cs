using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Utility.Nodes.Meta;
using Utility.Models;
using Utility.Interfaces.Exs;
using Utility.Trees.Extensions.Async;
using Splat;
using System.Reactive.Subjects;
using Utility.Helpers;
using Utility.PropertyNotifications;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using System.ComponentModel;

namespace Utility.Nodes.Demo.Filters.Services
{
    public enum ControlEventType
    {
        None, Save, Refresh
    }
    public readonly record struct ControlEvent(ControlEventType ControlEventType, int Count);

    public class ControlsService : IObservable<ControlEvent>
    {
        Dictionary<Guid, IDisposable> disposables = new();
        ReplaySubject<ControlEvent> replaySubject = new(1);
        Dictionary<ControlEventType, int> dict = new();

        private ControlsService()
        {
            Locator.Current.GetService<IObservableIndex<INodeViewModel>>()
                [nameof(NodeMethodFactory.BuildControlRoot)]
                .Subscribe(_n =>
                {
                    _n.Descendants()
                    .Subscribe(node =>
                    {
                        if (node.NewItem is IGetName { Name: string name } model and INotifyPropertyChanged changed)
                        {
                            changed.WhenChanged().Where(a => a.Name == ".ctor").Subscribe(_ =>
                            {
                                Switch(name, model);
                            });
                        }
                    });
                });
        }

        private void Switch(string name, IGetName model)
        {

            switch (name)
            {
                case NodeMethodFactory.Save:
                    var _ = Locator.Current.GetService<MethodCache>().Get(nameof(NodeMethodFactory.BuildRoot)).Subscribe(root =>
                    {
                        root
                        .Descendant(a => a.tree.ToString() == NodeMethodFactory.content_root)
                        .Subscribe(contentRoot =>
                        {
                            Locator.Current.GetService<INodeSource>().Save(); //((INode)contentRoot.NewItem);
                            dict[ControlEventType.Save] = DictionaryHelper.Get(dict, ControlEventType.Save) + 1;
                            replaySubject.OnNext(new ControlEvent(ControlEventType.Save, DictionaryHelper.Get<ControlEventType, int>(dict, ControlEventType.Save)));
                        });
                    });

                    break;

                case NodeMethodFactory.Refresh:
                    dict[ControlEventType.Refresh] = dict.Get(ControlEventType.Refresh) + 1;
                    //Refresh(contentRoot);
                    replaySubject.OnNext(new ControlEvent(ControlEventType.Refresh, dict.Get(ControlEventType.Refresh)));

                    break;
            }
        }

        public IDisposable Subscribe(IObserver<ControlEvent> observer)
        {
            return replaySubject.Subscribe(observer);
        }

        public static ControlsService Instance { get; } = new ControlsService();

    }

}
