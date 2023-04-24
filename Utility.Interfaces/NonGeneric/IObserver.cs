using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interfaces.NonGeneric
{
    public interface IObserver : IEquatable
    {
        //public List<object> Observations { get; }
        void OnNext(object value);

        void OnCompleted();

        void OnError(Exception error);
    }
}
