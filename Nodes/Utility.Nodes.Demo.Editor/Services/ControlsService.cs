using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Utility.Nodes.Filters;
using Utility.Models;
using Utility.Interfaces.Exs;
using Utility.Trees.Extensions.Async;

namespace Utility.Nodes.Demo.Filters.Services
{
    public class ControlsService
    {
        Dictionary<Guid, IDisposable> disposables = new();

        public ControlsService()
        {
            NodeSource.Instance
                .Many(nameof(Factory.BuildRoot))
                .Subscribe(root =>
                {
                    NodeSource.Instance.Single(nameof(Factory.BuildControlRoot))
                    .Subscribe(_n =>
                    {
                        _n.Descendants()
                        .Subscribe(node =>
                        {
                            if (node.NewItem is INode { Data: Model { Name: string name } model })
                            {
                                model.WhenAnyPropertyChanged().Subscribe(_ =>
                                {
                                    var contentRoot = root.Descendant(a => a.tree.Data.ToString() == Factory.content_root);
                                    Switch(name, model, contentRoot as INode);
                                });
                            }
                        });                 
                    });
                });
        }

        private void Switch(string name, Model model, INode? contentRoot)
        {
            switch (name)
            {
                case Factory.Save:
                    Save(contentRoot);
                    break;

                case Factory.Refresh:
                    //Refresh(contentRoot);
                    break;
            }
        }

   
        public void Save(INode node)
        {
            NodeSource.Instance.Save();
       
        }

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
