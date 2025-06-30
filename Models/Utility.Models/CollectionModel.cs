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
using Newtonsoft.Json;

namespace Utility.Models
{
    public interface IChildCollection
    {
        IEnumerable Collection { get; }
    }

    public interface ICollectionItem
    {
    }

    public abstract class BaseCollectionModel<TValue> : Model<TValue>, IChildCollection 
    {
        public virtual IEnumerable Collection { get; }
    }

    public class CollectionModel<TValue, T>(Func<T>? create = null) : BaseCollectionModel<TValue> 
    {
        private int limit = int.MaxValue;
        private readonly Func<T>? create = create;

        public override ObservableCollection<T> Collection { get; } = [];


        public virtual void Initialise()
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

            Collection
                .AndAdditions()
                .Subscribe(e =>
                {
                    Node.Add(e);
                });
        }

        [JsonIgnore]
        public virtual T New
        {
            get
            {
                if (this.create == default)
                    throw new Exception("33 RGEg");
                return create.Invoke();
            }
        }

        public override void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            if (Collection.Contains((T)a.Data) == false)
                Collection.Add((T)a.Data);
            base.Addition(value, a);
        }

        public override void Subtraction(IReadOnlyTree value, IReadOnlyTree a)
        {
            Collection.Remove((T)a.Data);
            base.Subtraction(value, a);
        }

        public override void SetNode(INode node)
        {
            node.IsAugmentable = true;
            Initialise();
            base.SetNode(node);
        }


        public int Limit { get => limit; set => RaisePropertyChanged(ref limit, value); }
    }

    public class CollectionModel(Func<Model>? create = null) : CollectionModel<Model>(create)
    {

    }
    public class CollectionModel<TModel>(Func<TModel>? create = null) : CollectionModel<object, TModel>(create) where TModel : class
    {

    }

    public class CollectionRootModel : CollectionModel
    {

    }
}
