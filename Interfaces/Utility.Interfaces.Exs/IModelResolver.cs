using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Exs
{
    public interface IModelResolver: IObservable<IModel>
    {
        void Register(string key, Func<IModel> factory);

        IModel this[string key] {get;}
    }
}
