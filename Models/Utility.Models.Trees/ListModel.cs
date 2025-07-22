using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;

namespace Utility.Models.Trees
{
    public class ListModel(Type type, Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null, Action<IValueModel>? attach = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : Model<IId<Guid>>(func, nodeAction, addition, attach, raisePropertyCalled, raisePropertyReceived)
    {
        private IEnumerable collection;
        private IId<Guid> add;
        private IId<Guid> remove;

        public IEnumerable Collection { get => collection; set => RaisePropertyChanged(ref collection, value); }

        public IId<Guid> New
        {
            get
            {

                var factory = Locator.Current.GetService<IFactory<IId<Guid>>>();
                var c = factory.Create(type);
                return c;

            }
 
        }

        public IId<Guid> Add
        {
            get => add;
            set { this.add = value; this.RaisePropertyReceived(value, null); }

        }
        public IId<Guid> Remove
        {
            get => remove;
            set { this.remove = value; this.RaisePropertyReceived(value, null); }

        }

    }

}
