using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility.Exceptions;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;

namespace Utility.Models.Trees
{
    public class ListModel<T> : ListModel
    {
        public ListModel(Func<IEnumerable<IReadOnlyTree>>? func = null, Action<INodeViewModel>? nodeAction = null, Action<IReadOnlyTree>? addition = null, Action<ListModel>? attach = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true)
            : base(typeof(T), func, nodeAction, addition, attach, raisePropertyCalled, raisePropertyReceived)
        {
        }
    }

    public class ListModel : Model<IId<Guid>>, IGetType, IProliferation
    {
        private Type type;
        private IEnumerable collection;
        private IId<Guid> add;
        private IId<Guid> remove;
        private int limit;

        public ListModel(Type type, Func<IEnumerable<IReadOnlyTree>>? func = null, Action<INodeViewModel>? nodeAction = null, Action<IReadOnlyTree>? addition = null, Action<ListModel>? attach = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : base(func, addition, a => attach?.Invoke((ListModel)a), raisePropertyCalled, raisePropertyReceived)
        {
            this.type = type;
            IsAugmentable = true;
            Orientation = Enums.Orientation.Vertical;
            this.WithChangesTo(a => a.Limit)
                .Subscribe(a =>
                {
                    this.Children.AndChanges<IReadOnlyTree>().Subscribe(c =>
                    {
                        Helpers.Generic.LinqEx.RemoveTypeOf<LimitExceededException, Exception>(this.Errors);
                        var count = this.Count(a => a is IRemoved { Removed: null });
                        this.IsAugmentable = count < a;
                        if (count > a)
                            this.Errors.Add(new LimitExceededException(a, count - a));
                    });

                });
        }

        public int Limit { get => limit; set => RaisePropertyReceived(ref limit, value); }

        public IEnumerable Collection { get => collection; set => RaisePropertyChanged(ref collection, value); }

        public override object Data { get => type; set => base.Data = value; }

        public object Start { get; set; }
        public object End { get; set; }

        public string XAxis { get; set; }

        public string YAxis { get; set; }

        public new IId<Guid> Add
        {
            get => add;
            set { this.add = value; this.RaisePropertyReceived(value, null); }
        }

        public new IId<Guid> Remove
        {
            get => remove;
            set { this.remove = value; this.RaisePropertyReceived(value, null); }

        }

        public virtual IEnumerable Proliferation()
        {
            var factory = Locator.Current.GetService<IFactory<IId<Guid>>>();
            return new IId<Guid>[] { factory.Create(type) };
        }
    }
}
