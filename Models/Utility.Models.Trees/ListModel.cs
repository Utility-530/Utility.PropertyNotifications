using DryIoc.ImTools;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;

namespace Utility.Models.Trees
{
    public class ListModel(Type type, Func<IEnumerable<IReadOnlyTree>>? func = null, Action<INodeViewModel>? nodeAction = null, Action<IReadOnlyTree>? addition = null, Action<ListModel>? attach = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) :
        Model<IId<Guid>>(func, nodeAction, addition, a => attach?.Invoke((ListModel)a), raisePropertyCalled, raisePropertyReceived),
        IGetType
    {
        private Type type = type;
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

        public override object Data { get => type; set => this.type = value is Type type ? type : throw new NotImplementedException("ds dd2111"); }

        public new IId<Guid> Add
        {
            get => add;
            set { this.add = value; this.RaisePropertyChanged(value, null); }
        }

        public new IId<Guid> Remove
        {
            get => remove;
            set { this.remove = value; this.RaisePropertyChanged(value, null); }

        }
    }
}
