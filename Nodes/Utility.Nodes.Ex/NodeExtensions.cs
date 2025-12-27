using System.Reactive.Disposables;
using System.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Observables.Generic;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Ex
{
    public static class NodeExtensions
    {
        public static string Name(this INodeViewModel node)
        {
            if (node is IGetName getName)
            {
                return getName.Name;
            }
            return node.ToString();
        }

        public static ITree ToNodeViewModel(this Assembly[] assemblies, Predicate<Type>? typePredicate = null)
        {
            NodeViewModel t_tree = new() { Name = "root" };

            foreach (var assembly in assemblies)
            {
                NodeViewModel tree = AssemblyModel.Create(assembly);

                foreach (var type in assembly.GetTypes())
                {
                    if (typePredicate?.Invoke(type) == false)
                        continue;
                    var _tree = new Model() 
                    {
                        Data = type,
                        Name = type.Name,
                        Parent = tree
                    };
                    tree.Add(_tree);
                }
                t_tree.Add(tree);
            }
            return t_tree;
        }

        public static ITree ToNodeViewModel(this Model<string>[] models, Predicate<Type>? typePredicate = null, INodeViewModel? t_tree = null)
        {
            t_tree ??= new NodeViewModel("root");

            foreach (var model in models)
            {
                model.WhenReceivedFrom(a => a.IsExpanded).Subscribe(isExpanded =>
                {
                    if (isExpanded)
                        ToNodeViewModel([.. model.Items().OfType<Model<string>>()], t_tree: model);
                });

                t_tree.Add(model);
            }
            return t_tree;
        }

        /// <summary>
        /// Creates a basic copy of <see cref="tree"/> with commands of the copy linked to the original
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static INodeViewModel Abstract(this INodeViewModel tree, out IDisposable disposables)
        {
            var _name = tree is IGetName { Name: { } name } ? name : tree.ToString();
            var clone = new NodeViewModel() { Name = _name, Key = (tree as IGetKey).Key, AddCommand = tree.AddCommand, RemoveCommand = tree.RemoveCommand, Removed = tree.Removed };
            var c_disposables = new CompositeDisposable();

            tree.WhenReceivedFrom(a => a.Removed).Subscribe(a => clone.Removed = a).DisposeWith(c_disposables);
            tree.WhenReceivedFrom(a => a.IsExpanded).Subscribe(a => clone.IsExpanded = a).DisposeWith(c_disposables);
            clone.WhenReceivedFrom(a => a.IsExpanded).Subscribe(a => tree.IsExpanded = a).DisposeWith(c_disposables);

            tree.Children.AndAdditions<INodeViewModel>().Subscribe(async item =>
            {
                var childClone = (ITree)item.Abstract(out var _disposables);
                (c_disposables).Add(_disposables);
                (childClone as ISetParent<IReadOnlyTree>).Parent = clone;
                clone.Add(childClone);
            }).DisposeWith(c_disposables);

            disposables = c_disposables;
            //list.Add(disposables);
            return clone;
        }
    }

    public class Abstract
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}