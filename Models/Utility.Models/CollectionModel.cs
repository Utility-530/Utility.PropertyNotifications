using Utility.Trees.Abstractions;
using System.Collections.ObjectModel;
using System.Collections;
using Utility.Interfaces.Exs;
using Utility.PropertyNotifications;
using Utility.Reactives;
using System.Reactive.Linq;
using Utility.Exceptions;
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

    public abstract class BaseCollectionModel<TValue, TR> : Model<TValue, TR>, IChildCollection 
    {
        public BaseCollectionModel(Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null, Action<TR>? attach = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) :
            base(func, nodeAction, addition, attach, raisePropertyCalled, raisePropertyReceived)
        {
        }
        public virtual IEnumerable Collection { get; }
    }

    public class CollectionModel<TValue, T, TR> : BaseCollectionModel<TValue, TR> 
    {
        private int limit = int.MaxValue;
        private Func<T>? create;

        public CollectionModel(Func<T>? create = null, Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null, Action<TR>? attach = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) :
        base(func, nodeAction, addition, attach, raisePropertyCalled, raisePropertyReceived)
        {
            this.create = create;
        }

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

    public class CollectionModel(Func<Model>? create = null) : CollectionModel<Model, CollectionModel>(create)
    {

    }
    public class CollectionModel<TModel>(Func<Model>? create = null) : CollectionModel<Model, CollectionModel<TModel>>(create)
    {

    }
    public class CollectionModel<TModel, TAttach>(Func<TModel>? create = null, Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null, Action<TAttach>? attach = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : CollectionModel<object, TModel, TAttach>(create, func, nodeAction, addition, attach, raisePropertyCalled, raisePropertyReceived) where TModel : class
    {
    }

    public class CollectionRootModel : CollectionModel<CollectionRootModel>
    {

    }
}
