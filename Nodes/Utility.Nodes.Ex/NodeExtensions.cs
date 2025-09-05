using System.Reactive.Disposables;
using System.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Trees;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Ex
{
    public static class NodeExtensions
    {
        public static string Name(this INode node)
        {
            if (node.Data is IGetName getName)
            {
                return getName.Name;
            }
            return node.Data.ToString();
        }

        public static ITree ToViewModelTree(this Assembly[] assemblies, Predicate<Type>? typePredicate = null)
        {
            ViewModelTree t_tree = new("root");

            foreach (var assembly in assemblies)
            {

                ViewModelTree tree = new(AssemblyModel.Create(assembly));

                foreach (var type in assembly.GetTypes())
                {
                    if (typePredicate?.Invoke(type) == false)
                        continue;
                    var _tree = new ViewModelTree(new TypeModel { Name = type.Name, Type = type })
                    {
                        Parent = tree
                    };
                    tree.Add(_tree);
                }
                t_tree.Add(tree);
            }
            return t_tree;
        }

        public static ITree ToViewModelTree(this ReadOnlyStringModel[] models, Predicate<Type>? typePredicate = null, INode? t_tree = null)
        {
            t_tree ??= new Node("root");

            foreach (var model in models)
            {

                Node tree = new(model) { HasItems = true };
                model.Node = tree;
                tree.WithChangesTo(a => a.IsExpanded).Subscribe(isExpanded =>
                {
                    if (isExpanded)
                        ToViewModelTree([.. model.Children.OfType<ReadOnlyStringModel>()], t_tree: tree);
                });

                t_tree.Add(tree);
            }
            return t_tree;
        }

        /// <summary>
        /// Creates a basic copy of <see cref="tree"/> with commands of the copy linked to the original
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static INode Abstract(this INode tree, out IDisposable disposables)
        {
            var _name = tree.Data is IGetName { Name: { } name } ? name : tree.Data.ToString();
            var clone = new Node(new Abstract { Name = _name }) { Key = (tree as IGetKey).Key, AddCommand = tree.AddCommand, RemoveCommand = tree.RemoveCommand, Removed = tree.Removed };
            var c_disposables = new CompositeDisposable();

            tree.WithChangesTo(a => a.Removed).Subscribe(a => clone.Removed = a).DisposeWith(c_disposables);
            tree.WithChangesTo(a => a.IsExpanded).Subscribe(a => clone.IsExpanded = a).DisposeWith(c_disposables);
            clone.WithChangesTo(a => a.IsExpanded).Subscribe(a => tree.IsExpanded = a).DisposeWith(c_disposables);

            tree.Items.AndAdditions<Node>().Subscribe(async item =>
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
