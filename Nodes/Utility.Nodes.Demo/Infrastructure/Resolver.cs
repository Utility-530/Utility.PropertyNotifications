using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Infrastructure
{
    public class Resolver
    {
        public Tree<Type> tree = new(typeof(TopViewModel));

        public Resolver()
        {
            Register<TopViewModel, BreadcrumbsViewModel>();
            Register<BreadcrumbsViewModel, RootViewModel>();
            Register<BreadcrumbsViewModel, DescendantsViewModel>();
        }

        //public object Resolve(Type type)
        //{
        //    return Activator.CreateInstance(type);
        //}

        public Type Root => tree.Data;

        public IEnumerable Children<T>()
        {
            return Children(typeof(T));
        }

        public IEnumerable Children(Type type)
        {
            if (tree.Match(type) is var branch)
                return branch.Items.Cast<IReadOnlyTree>().Select(a => a.Data).ToArray();
            return Array.Empty<Type>();
        }

        public void Register<TParent, TChild>()
        {
            tree[typeof(TParent)].Add(typeof(TChild));
        }

        public void Register(Type parent, Type child)
        {
            tree[parent].Add(child);
        }

        public static Resolver Instance { get; } = new Resolver();
    }

    public abstract class ViewModel
    {
        public abstract PackIconKind Icon { get; }
        public abstract string Name { get; }
    }

    public class TopViewModel : ViewModel
    {
        public override string Name => "Top";

        public override PackIconKind Icon => PackIconKind.AlignVerticalTop;
    }

    public class BreadcrumbsViewModel : ViewModel
    {
        public override string Name => "Breadcrumbs";

        public override PackIconKind Icon => PackIconKind.Bread;
    }
    public class RootViewModel : ViewModel
    {
        public override string Name => "Root";

        public override PackIconKind Icon => PackIconKind.SquareRoot;
    }
    public class DescendantsViewModel : ViewModel
    {
        public override string Name => "Descendants";

        public override PackIconKind Icon => PackIconKind.OrderBoolDescending;
    }
}
