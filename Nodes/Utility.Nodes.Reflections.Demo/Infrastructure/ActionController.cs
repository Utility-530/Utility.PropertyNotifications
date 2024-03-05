using Splat;

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
            var collectionDescriptor = tree.MatchAncestor(a => a.Data is CollectionDescriptor).Parent;
            var node = collectionDescriptor.MatchDescendant(a => a.Data is MethodDescriptor { Name: nameof(IList.Add) } node) as ReflectionNode;
            node.VisitDescendants(a => { });
            var items = node.Items;
            (node.Data as MethodDescriptor).Invoke();
        }

        public void Duplicate(IReadOnlyTree tree)
        {
            if (tree.Data is IMemberDescriptor memberDescriptor)
            {
                var repo = Locator.Current.GetService<ITreeRepository>();
                foreach (var (a, b) in repo.Duplicate(memberDescriptor.Guid))
                {
                    repo.Copy(a, b);
                }
            }
            var collectionDescriptor = tree.MatchAncestor(a => a.Data is CollectionDescriptor).Parent;
            var methodsDescriptor = collectionDescriptor.MatchDescendant(a => a.Data is MethodsDescriptor);
            var node = methodsDescriptor.MatchDescendant(a => a.Data is MethodDescriptor { Name: nameof(IList.Add) } node) as ReflectionNode;
            var methodDescriptor = (node.Data as MethodDescriptor);
            methodDescriptor[0] = (tree.Data as PropertyDescriptor).Instance;
            methodDescriptor.Invoke();
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
