using Utility.PropertyTrees.Abstractions;
using System.Reactive.Subjects;
using Utility.Trees;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.PropertyTrees.Infrastructure
{
    public class Order : BaseViewModel, IObservable<PropertyChange>, IKey
    {
        private Exception exception;
        private int progress;
        Subject<PropertyChange> changes = new();

        public Key Key { get; set; }
        public HistoryState State { get; set; }
        public OrderType OrderType { get; set; }
        public object Value { get; set; }
        public int Progress
        { get => progress; set { progress = value; OnPropertyChanged(); } }

        public Exception Exception
        {
            get => exception; set { exception = value; OnPropertyChanged(); }
        }

        public bool Equals(IKey? other)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<PropertyChange> observer)
        {
            return changes.Subscribe(observer);
        }
    }
}