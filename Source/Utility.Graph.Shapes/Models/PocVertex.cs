using Jellyfish;
using MintPlayer.ObservableCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Utility.Commands;
using Utility.Infrastructure;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.NonGeneric;

namespace Utility.Graph.Shapes
{
    /// <summary>
    /// A simple identifiable vertex.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ID) + "}")]
    public class PocVertex : ObservableObject,  IEquatable<PocVertex>, IObservable
    {
        private int broadcast;
        private PocVertex selectedVertex;
        private List<IObserver> observers = new();

        //public event Action Visited;
        public List<ViewModelEvent> viewModelEvents = new();
        
        public PocVertex(string id)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));

            SelectCommand = new Command(() =>
            {
                var selectEvent = new SelectEvent(this);
                viewModelEvents.Add(selectEvent);
                foreach(var observer in Observers)
                {
                    observer.OnNext(selectEvent);   
                }              
            });

            BreakCommand = new Command(() =>
            {
                var breakEvent = new BreakEvent(this);
                viewModelEvents.Add(breakEvent);
                foreach (var observer in Observers)
                {
                    observer.OnNext(breakEvent);   
                }              
            });

            Events.CollectionChanged += Events_CollectionChanged;
        }

        private void Events_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LastEvent = Events.LastOrDefault();
            this.OnPropertyChanged(nameof(LastEvent));  
        }

        public ICommand SelectCommand { get; }
        public ICommand BreakCommand { get; }

        public string ID { get; }

        public PocVertex SelectedVertex { get => selectedVertex; set => this.Set(ref selectedVertex, value); }

        public Event LastEvent { get; set; }

        public ObservableCollection<Event> Events { get; } = new();


        public IEnumerable<IObserver> Observers => observers;

        public IDisposable Subscribe(IObserver observer)
        {
            return new Disposer(observers, observer);
        }

        public IEnumerator GetEnumerator()
        {
            return viewModelEvents.GetEnumerator();
        }

        #region 
        /// <inheritdoc />
        public override string ToString()
        {
            return ID;
        }

        public bool Equals(PocVertex? other)
        {
            return (this.ID == other?.ID);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PocVertex);
        }

        public bool OnNext(object value)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public record ViewModelEvent();

    public record SelectEvent(PocVertex Vertex) : ViewModelEvent;
    public record BreakEvent(PocVertex Vertex) : ViewModelEvent;
}