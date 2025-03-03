using Utility.Trees.Abstractions;
using System.Collections.ObjectModel;

namespace Utility.Models
{
    public interface IChildCollection
    {
    }

    public interface ICollectionItem
    {
    }

    public class CollectionModel<T> : Model, IChildCollection
    {
        public ObservableCollection<T> Collection { get; } = [];

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            Collection.Add((T)a.Data);
            base.Addition(value, a);
        }
    }
}
