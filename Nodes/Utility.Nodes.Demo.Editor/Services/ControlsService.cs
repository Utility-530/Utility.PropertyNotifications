using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Utility.Nodes.Filters;
using Utility.Models;
using Utility.Interfaces.Exs;
using Utility.Trees.Extensions.Async;
using Splat;
using System.Reactive.Subjects;
using Utility.Helpers;
using Utility.PropertyNotifications;

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

        private ControlsService()
        {
            Locator.Current.GetService<INodeSource>().Single(nameof(Factory.BuildControlRoot))
            .Subscribe(_n =>
            {
                _n.Descendants()
                .Subscribe(node =>
                {
                    if (node.NewItem is INode { Data: Model { Name: string name } model })
                    {
                        model.WithChanges(includeInitialValue: false).Subscribe(_ =>
                        {
                            //
                            Switch(name, model);
                        });
                    }
                });
            });
        }

        Dictionary<ControlEventType, int> dict = new();

        private void Switch(string name, Model model)
        {

            switch (name)
            {
                case Factory.Save:
                    var _ = Locator.Current.GetService<INodeSource>().Single(nameof(Factory.BuildRoot)).Subscribe(root =>
                    {
                        root.Descendant(a => a.tree.Data.ToString() == Factory.content_root).Subscribe(contentRoot =>
                        {
                            Save((INode)contentRoot.NewItem);
                            dict[ControlEventType.Save] = dict.Get(ControlEventType.Save) + 1;
                            replaySubject.OnNext(new ControlEvent(ControlEventType.Save, dict.Get(ControlEventType.Save)));
                        });
                    });

                    break;

                case Factory.Refresh:
                    dict[ControlEventType.Refresh] = dict.Get(ControlEventType.Refresh) + 1;
                    //Refresh(contentRoot);
                    replaySubject.OnNext(new ControlEvent(ControlEventType.Refresh, dict.Get(ControlEventType.Refresh)));

                    break;
            }
        }


        public void Save(INode node)
        {
            Locator.Current.GetService<INodeSource>().Save();

        }

        public IDisposable Subscribe(IObserver<ControlEvent> observer)
        {
            return replaySubject.Subscribe(observer);
        }

        public static ControlsService Instance { get; } = new ControlsService();

    }

    public static class Helpers
    {
        public static IEnumerable<T> Select<T>(this ArrayList list, Func<object, T> map)
        {
            foreach (var x in list)
            {
                yield return map(x);
            }
        }

        public static HashSet<T> ToHashSet<T>(Collection<T> collection)
        {
            return new HashSet<T>(collection);
        }
    }
}
