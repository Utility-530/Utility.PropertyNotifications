using Utility.Trees.Abstractions;
using System.Collections.ObjectModel;
using System.Collections;
using Utility.Interfaces.Exs;
using Utility.PropertyNotifications;
using Utility.Reactives;
using System.Reactive.Linq;
using Utility.Exceptions;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;

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
        private int limit = int.MaxValue;

        public override ObservableCollection<T> Collection { get; } = [];

        public CollectionModel()
        {
            this.WithChangesTo(a => a.Limit)
                .CombineLatest(this.WithChangesTo(a => a.Node, includeNulls: false))
                .Subscribe(a =>
                {
                    a.Second.Items.AndChanges<IReadOnlyTree>().Subscribe(c =>
                    {
                        Helpers.Generic.LinqEx.RemoveTypeOf<LimitExceededException, Exception>(Node.Errors);
                        var count = a.Second.Count(a => a is IRemoved { Removed: null });
                        a.Second.IsAugmentable = count < a.First;
                        if (count > a.First)
                            Node.Errors.Add(new LimitExceededException(a.First, count - a.First));
                    });
                });
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            Collection.Add((T)a.Data);
            base.Addition(value, a);
        }

        public override void SetNode(INode node)
        {
            node.IsAugmentable = true;
            base.SetNode(node);
        }


        public int Limit { get => limit; set => RaisePropertyChanged(ref limit, value); }
    }

    public class CollectionModel : CollectionModel<Model>
    {

    }

    public class CollectionRootModel : CollectionModel
    {

    }
}
