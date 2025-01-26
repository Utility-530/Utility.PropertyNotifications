using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using AutoMapper;
using DynamicData;
using DynamicData.Binding;
using Splat;
using Utility.Extensions;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Filters;
using Utility.Repos;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Interfaces.Exs;
using Utility.Trees.Extensions;

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
                        foreach (INode INode in _n)
                        {
                            if (INode is { Data: Model { Name: string name } model })
                            {
                                model.WhenAnyPropertyChanged().Subscribe(_ =>
                                {
                                    var contentRoot = root.Descendant(a => (a.tree as IName).Name == Factory.content_root);
                                    Switch(name, model, contentRoot as INode);
                                });
                            }
                        }
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
