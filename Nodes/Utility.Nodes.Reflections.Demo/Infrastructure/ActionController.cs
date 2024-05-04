using Splat;
using Utility.Extensions;
using Utility.Interfaces;

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
            var collectionDescriptor = tree.MatchAncestor(a => a.Data is ICollectionDescriptor).Parent;
            var node = collectionDescriptor.MatchDescendant(a => a.Data is IMethodDescriptor { Name: nameof(IList.Add) } node) as ReflectionNode;
            node.VisitDescendants(a => { });
            var items = node.Items;
            (node.Data as IMethodDescriptor).Invoke();
        }

        public void Duplicate(IReadOnlyTree tree)
        {
            if (tree.Data is IDescriptor memberDescriptor)
            {
                var repo = Locator.Current.GetService<ITreeRepository>();
                foreach (var (a, b) in repo.Duplicate(memberDescriptor.Guid))
                {
                    repo.Copy(a, b);
                }
            }
            var collectionDescriptor = tree.MatchAncestor(a => a.Data is ICollectionDescriptor).Parent;
            var methodsDescriptor = collectionDescriptor.MatchDescendant(a => a.Data is IMethodsDescriptor);
            var node = methodsDescriptor.MatchDescendant(a => a.Data is IMethodDescriptor { Name: nameof(IList.Add) } node) as ReflectionNode;
            var methodDescriptor = (node.Data as IMethodDescriptor);
            methodDescriptor[0] = (tree.Data as IInstance).Instance;
            methodDescriptor.Invoke();
        }

        public void Remove(IReadOnlyTree tree)
        {
            var collectionDescriptor = tree.MatchAncestor(a => a.Data is ICollectionDescriptor).Parent;
            var methodsDescriptor = collectionDescriptor.MatchDescendant(a => a.Data is IMethodsDescriptor);
            var node = methodsDescriptor.MatchDescendant(a => a.Data is IMethodDescriptor { Name: nameof(IList.Remove) } node) as ReflectionNode;
            var methodDescriptor = (node.Data as IMethodDescriptor);
            methodDescriptor[0] = (tree.Data as IInstance).Instance;
            methodDescriptor.Invoke();
        }
    }
}
