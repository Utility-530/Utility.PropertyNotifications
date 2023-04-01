using System.Collections.ObjectModel;
using Utility.Observables;

namespace Utility.Trees
{



    public class DynamicTree<T> : IObservable<ITree<T>>, IObserver<ITree<T>>
    {
        private new ObservableCollection<ITree<T>> children => Current.Parent?.Items as ObservableCollection<ITree<T>>;
        private ITree<T> tree;
        private List<IObserver<ITree<T>>> observers = new();
        private ITree<T> toRemove;
        private ITree<T> current;
        private ITree<T> toAdd;
        private State state = State.Current;

        //public TreeState TreeState() => new(
        //    Index,
        //    State,
        //    Current,
        //    CanMoveForward ? Forward : default,
        //    CanMoveBack ? Back : default,
        //    CanMoveUp ? Up : default,
        //    CanMoveDown ? Down : default,
        //    CanAdd ? ToAdd : default,
        //    CanRemove ? ToRemove : default);


        public DynamicTree(ITree<T> root)
        {
            Current = tree = root;
        }

        public bool CanMoveUp => Current.Parent != null;
        public bool CanMoveDown => Current.HasItems;
        public bool CanMoveForward => Index < children?.Count - 1;

        public bool CanMoveBack => Index > 0;
        public bool CanAdd => ToAdd != null;
        public bool CanRemove => Current?.Parent != null;


        public ITree<T> Up => Current.Parent;

        public ITree<T> Down => Current[0];

        public ITree<T> Forward => children[Index + 1];

        public ITree<T> Back => children[Index - 1];

        public ITree<T> Current
        {
            get
            {
                return current ?? this.Tree;
            }

            private set => current = value;
        }

        public State Allowed
        {
            get
            {
                State state = State.Default;
                if (CanMoveUp)
                    state |= State.Up;
                if (CanMoveDown)
                    state |= State.Down;
                if (CanMoveBack)
                    state |= State.Back;
                if (CanMoveForward)
                    state |= State.Forward;
                if (CanAdd)
                    state |= State.Add;  
                if (CanRemove)
                    state |= State.Remove;
                return state;
            }
        }


        public ITree<T> ToRemove => toRemove;

        public ITree<T> ToAdd => toAdd;

        public T Data { get => toAdd.Data; set => toAdd = new Tree<T>(value) { Parent = Current, Key = Guid.NewGuid() }; }

        public State State
        {
            get => state;
            set
            {
                state = value;
                Current.State = State.Default;
                switch (value)
                {

                    case State.Forward:
                        if (CanMoveForward == false)
                            throw new Exception($"{State.Forward} V3 fds");
                        Current = Forward;
                        Current.State = State.Forward;
                        break;
                    case State.Back:
                        if (CanMoveBack == false)
                            throw new Exception($"{State.Back} V3 fds");
                        Current = Back;
                        Current.State = State.Back;
                        break;
                    case State.Down:
                        if (CanMoveDown == false)
                            throw new Exception($"{State.Down} V3 fds");
                        Current = Down;
                        Current.State = State.Back;
                        break;
                    case State.Up:
                        if (CanMoveUp == false)
                            throw new Exception($"{State.Up} V3 fds");
                        Current = Up;
                        Current.State = State.Back;
                        break;
                    case State.Add:
                        if (CanAdd == false)
                            throw new Exception($"{State.Add} V3 fds");
                        Add();
                        Current.State = State.Add;
                        break;
                    case State.Remove:
                        if (CanRemove == false)
                            throw new Exception($"{State.Remove} V3 fds");
                        Remove();
                        Current.State = State.Remove;
                        break;

                }
                Broadcast();
                Current.State = value;

                void Add()
                {
                    toRemove = ToAdd;
                    Current.Add(toRemove);
                    Current = toRemove;
                }

                void Remove()
                {
                    var parent = Current.Parent;
                    Current.Parent.Remove(Current);
                    Current = parent;
                }
            }
        }
        public ITree<T> Tree => tree;

        public int Index => Current.Parent?.IndexOf(Current) ?? 0;


        public void Dispose()
        {
            children.Clear();
        }


        public void Broadcast()
        {
            foreach (var observer in observers)
                observer.OnNext(Current);
        }

        public IDisposable Subscribe(IObserver<ITree<T>> observer)
        {
            return new Disposer<ITree<T>>(observers, observer);
        }

        public ITree<T> Last()
        {
            return children.LastOrDefault();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ITree<T> value)
        {
            //index = value.Index;
            //var current = value.Data as TreeState;

            //if (tree.Match(current.Current.Key) is null)
            //{
            //    throw new Exception("rg 4!£");
            //}
            //State = State.Default;
            Current = value as ITree<T>;
            //toRemove = value.Remove as ITree<T>;
            Broadcast();
        }

        //public void OnPrevious(TreeState data)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
