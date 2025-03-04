using Utility.Trees.Abstractions;
using System.Collections.ObjectModel;
using System.Collections;

namespace Utility.Models
{
    public interface IChildCollection
    {
        IEnumerable Collection { get; }
    }

    public interface ICollectionItem
    {
    }

    public abstract class CollectionModel : Model, IChildCollection
    {
        public virtual IEnumerable Collection { get; } 

    }

    public class CollectionModel<T> : CollectionModel
    {
        public override ObservableCollection<T> Collection { get; } = [];

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            Collection.Add((T)a.Data);
            base.Addition(value, a);
        }
    }
}
