using MoreLinq;
using System.Reactive.Linq;
using Utility.Trees.Abstractions;

namespace Utility.Trees.Extensions
{
    public static class Match
    {
        public static IReadOnlyTree? Root(this IReadOnlyTree tree) => Ancestors(tree, t => t.Item1.IsRoot()).SingleOrDefault();

        public static IReadOnlyTree? Ancestor(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            return SelfAndAncestors(tree, action).SingleOrDefault();
        }


        public static IEnumerable<IReadOnlyTree> Ancestors(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            return SelfAndAncestors(tree.Parent, action);
        }


        public static IEnumerable<IReadOnlyTree> SelfAndAncestors(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null, int level = 0)
        {
            action ??= n => true;
            if (action((tree, level)))
            {
                yield return tree;
            }

            if (tree?.Parent is IReadOnlyTree parent)
            {
                foreach (var x in SelfAndAncestors(parent, action, level++))
                    yield return x;
            }
        }

        public static IReadOnlyTree? Descendant(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            return SelfAndDescendants(tree, action).SingleOrDefault();
        }

        public static IEnumerable<IReadOnlyTree> Descendants(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            foreach (IReadOnlyTree item in tree.Items)
                foreach (var d in SelfAndDescendants(item, action))
                {
                    yield return d;
                }
        }    
        public static IEnumerable<IReadOnlyTree> SelfAndDescendants(this IReadOnlyTree tree, Predicate<(IReadOnlyTree, int)>? action = null, int level = 0)
        {
            action ??= n => true;
            if (action((tree, level)))
            {
                yield return tree;
            }
            level++;
            foreach (IReadOnlyTree item in tree.Items)
                foreach (var x in SelfAndDescendants(item, action, level))
                    yield return x;
        }
    }
}
