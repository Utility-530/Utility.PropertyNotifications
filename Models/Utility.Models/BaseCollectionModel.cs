using Utility.Trees.Abstractions;
using System.Collections.ObjectModel;
using System.Collections;
using Utility.Interfaces.Exs;

namespace Utility.Models
{
    public interface IChildCollection
    {
        IEnumerable Collection { get; }
    }

    public interface ICollectionItem
    {
    }

    public abstract class BaseCollectionModel : Model, IChildCollection
    {
        public virtual IEnumerable Collection { get; } 

    }

    public class CollectionModel<T> : BaseCollectionModel
    {
        public override ObservableCollection<T> Collection { get; } = [];

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            Collection.Add((T)a.Data);
            base.Addition(value, a);
        }

        public override void SetNode(INode node)
        {
            node.IsEditable = true; 
            base.SetNode(node);
        }
    }    
    
    public class CollectionModel : CollectionModel<Model>
    {

    }  
    
    public class CollectionRootModel : CollectionModel
    {

    }
}
