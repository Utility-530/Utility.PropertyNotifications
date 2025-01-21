using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.NonGeneric;
using Utility.Structs;

namespace Utility.Interfaces.Exs
{

    public interface IValueChanges : IObservable<ValueChange>
    {
    }

}
