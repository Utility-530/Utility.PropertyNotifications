using System;
using System.Collections;
using System.Linq;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;

namespace Utility.Nodes.Demo.Infrastructure
{
    public class Resolver
    {
        Tree<Type> root = new();

        private Resolver()
        { 
        }

        public Type RootType => (Type)root.Data;

        public IEnumerable Children<T>()
        {
            return Children(typeof(T));
        }

        public IEnumerable Children(Type type)
        {
            if (root.Descendant((a) => (Type)a.tree.Data == type) is IReadOnlyTree branch)
                return branch.Children.Cast<IReadOnlyTree>().Select(a => a.Data).ToArray();
            return Array.Empty<Type>();
        }

        public Resolver Register<TParent, TChild>()
        {
           return Register(typeof(TParent), typeof(TChild));
        }

        public Resolver Register(Type parent, Type child)
        {
            if (root.Descendant(a => (Type)a.tree.Data == parent) is Tree branch)
                branch.Add(child);
            else if (root.Children.Count == 0)
            {
                root.Data = parent;
                Register(parent, child);
            }
            else
                throw new Exception("£ £$££");
            return this;
        }

        public static Resolver Instance { get; } = new Resolver();
    }

}
