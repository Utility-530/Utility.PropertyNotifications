using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Observables
{
    public class Subject : IObservable, IObserver
    {
        public List<IObserver> Observers => throw new NotImplementedException();

        public List<object> Observations => throw new NotImplementedException();


        public void OnNext(object value)
        {
            Observations.Add(value);
            foreach (var observer in Observers)
                observer.OnNext(value);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
