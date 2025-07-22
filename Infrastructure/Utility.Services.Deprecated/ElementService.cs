using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Common.Contract;
using Utility.Common.Helper;
using Utility.Helpers.NonGeneric;

namespace Utility.Services.Deprecated
{
    public class ElementService<T> : IObserver<RepositoryMessage> where T : IEquatable<T>
    {
        private readonly ReplaySubject<RepositoryMessage> subject = new(1);
        private readonly ReplaySubject<T?> itemSubject = new(1);
        private T? value;

        public ElementService()
        {
            subject
                .Take(1)
                .Subscribe(a =>
                {
                    value = a.Service.First<T>();
                });

            subject
                .CombineLatest(itemSubject)
                .Subscribe(a =>
                {
                    if ((bool)a.First.Service.Update(a) == false)
                        a.First.Service.Add(a);
                });
        }

        public T? Value
        {
            get => value;
            set
            {
                this.value = value;
                itemSubject.OnNext(value);
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(RepositoryMessage value)
        {
            subject.OnNext(value);
        }
    }
}