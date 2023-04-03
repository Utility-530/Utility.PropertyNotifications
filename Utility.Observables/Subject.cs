using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Observables
{
    public class Subject : IObservable, IObserver
    {
        public List<IObserver> Observers { get; } = new();

        public List<object> Observations { get; } = new();


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

        public IEnumerator GetEnumerator()
        {
            return Observations.GetEnumerator();
        }
    }
}
