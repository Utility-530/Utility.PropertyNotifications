using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Observables
{
    public interface IObserver
    {
        public List<object> Observations { get; }
        void OnNext(object value)
        {
            Observations.Add(value);
        }

        void OnCompleted();

        void OnError(Exception error);
    }
}
