using Splat;
using System;
using System.Collections;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;

namespace Utility.Models.Trees
{
    public class ListModel(Type type) : Model<IId<Guid>>
    {
        private IEnumerable collection;
        private IId<Guid> add;
        private IId<Guid> remove;

        public IEnumerable Collection { get => collection; set => RaisePropertyChanged(ref collection, value); }

        public IId<Guid> New
        {
            get
            {

                var instance = Locator.Current.GetService<IFactory<IId<Guid>>>();
                var c = instance.Create(type);
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
