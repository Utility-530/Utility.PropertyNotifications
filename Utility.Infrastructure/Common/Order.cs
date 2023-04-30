using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Enums;
using Utility.Observables.Generic;
using Utility.Interfaces.Generic;
using Utility.PropertyTrees.Infrastructure;

namespace Utility.Infrastructure
{
    public class PropertyOrder : BaseViewModel, IObservable<PropertyChange>, IKey<Key>
    {
        private Exception exception;
        private int progress;
        Relay<PropertyChange> changes = new();

        public Key Key { get; set; }
        public Enums.History State { get; set; }
        public Access Access { get; set; }
        public object Value { get; set; }
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        public Exception Exception
        {
            get => exception;
            set
            {
                exception = value;
                OnPropertyChanged();
            }
        }

        public bool Equals(IKey<Key>? other)
        {
            return other?.Key.Equals(Key) == true;
        }

        public bool Equals(IEquatable? other)
        {
            return Equals(other as IKey<Key>);
        }

        public IDisposable Subscribe(IObserver<PropertyChange> observer)
        {
            return changes.Subscribe(observer);
        }
    }

}