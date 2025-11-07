//using System.Collections;
//using System.Reactive.Linq;
//using Utility.Exceptions;
//using Utility.Interfaces.Exs;
//using Utility.Interfaces.Generic;
//using Utility.Interfaces.NonGeneric;
//using Utility.Nodes;
//using Utility.PropertyNotifications;
//using Utility.Reactives;
//using Utility.Trees.Abstractions;

//namespace Utility.Models
//{
//    public interface IChildCollection
//    {
//        int Limit { get; set; }
//    }

//    public interface ICollectionItem
//    {
//    }

//    public abstract class BaseCollectionModel<TValue> : NodeViewModel, IChildCollection, IGet<TValue>, Interfaces.Generic.ISet<TValue>
//    {
//        private int limit = int.MaxValue;
//        private TValue? value;

//        public BaseCollectionModel() :
//            base()
//        {
//            this.Children
//                .AndAdditions<IReadOnlyTree>()
//                .Subscribe(a =>
//                {
//                    Addition(a);
//                });

//            this.Children
//                .Subtractions<IReadOnlyTree>()
//                .Subscribe(a =>
//                {
//                    Subtraction(a);
//                });
//            this.Children
//                .Replacements<IReadOnlyTree>()
//                .Subscribe(a =>
//                {
//                    Replacement(a.@new, a.old);
//                });
//        }

//        protected virtual void Replacement(IReadOnlyTree @new, IReadOnlyTree old)
//        {
//        }

//        protected virtual void Subtraction(IReadOnlyTree a)
//        {
//        }

//        protected virtual void Addition(IReadOnlyTree a)
//        {
//        }

//        public override object? Value
//        {
//            get { RaisePropertyCalled(value); return value; }
//            set => this.RaisePropertyReceived<TValue>(ref this.value, (TValue)value);
//        }

//        public TValue? Get()
//        {
//            return value;
//        }

//        public void Set(TValue value)
//        {
//            this.value = value;
//        }

//        public int Limit { get => limit; set => RaisePropertyChanged(ref limit, value); }
//    }

//    public class CustomCollectionModel<TValue, T> : CollectionModel<TValue, T> where TValue : new()
//    {
//    }

//    public class CollectionModel<TValue, T> : BaseCollectionModel<TValue>, IProliferation
//    {
//        private Func<TValue>? create;

//        public CollectionModel(Func<TValue>? create = null)
//        {
//            this.create = create;
//            this.IsAugmentable = true;
//            Initialise();
//        }

//        //public override ObservableCollection<T> Collection { get; } = [];

//        public void Initialise()
//        {
//            this.WithChangesTo(a => a.Limit)
//                .Subscribe(a =>
//                {
//                    this.Children.AndChanges<IReadOnlyTree>().Subscribe(c =>
//                    {
//                        Helpers.Generic.LinqEx.RemoveTypeOf<LimitExceededException, Exception>(this.Errors);
//                        var count = this.Count(a => a is IRemoved { Removed: null });
//                        this.IsAugmentable = count < a;
//                        if (count > a)
//                            this.Errors.Add(new LimitExceededException(a, count - a));
//                    });
//                });
//        }

//        public virtual IEnumerable Proliferation()
//        {
//            if (create is not null)
//                yield return create.Invoke();
//        }
//    }

//    public class CollectionModel<TModel>(Func<Model>? create = null) : ListModel
//    {
//    }
//    public class CollectionModel() : CollectionModel<NodeViewModel>()
//    {
//    }
//}