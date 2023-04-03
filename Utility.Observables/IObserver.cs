using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Observables
{
    public interface IObserver //: IKey
    {
        //public List<object> Observations { get; }
        void OnNext(object value);
    

        void OnCompleted();

        void OnError(Exception error);
    }
}
