using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.PropertyDescriptors;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Reflections.Demo.Infrastructure
{
    public class ActionController
    {
        public ActionController()
        {

        }

        public static ActionController Instance { get; } = new();

        public void Add(IReadOnlyTree tree)
        {
            var collectionDescriptor = tree.MatchAncestor(a => a.Data is CollectionDescriptor);
            var node = collectionDescriptor.MatchDescendant(a => a.Data is MethodDescriptor { Name: nameof(IList.Add) } node) as ReflectionNode;
            (node.Data as MethodDescriptor).Invoke();
        }

        public void Duplicate(IReadOnlyTree tree)
        {

        }

        public void Remove(IReadOnlyTree tree)
        {
            var collectionDescriptor = tree.MatchAncestor(a => a.Data is CollectionDescriptor).Parent;
            var methodsDescriptor = collectionDescriptor.MatchDescendant(a => a.Data is MethodsDescriptor);
            var node = methodsDescriptor.MatchDescendant(a => a.Data is MethodDescriptor { Name: nameof(IList.Remove) } node) as ReflectionNode;
            var methodDescriptor = (node.Data as MethodDescriptor);
            methodDescriptor[0] = (tree.Data as PropertyDescriptor).Instance;
            methodDescriptor.Invoke();
        }
    }
}
