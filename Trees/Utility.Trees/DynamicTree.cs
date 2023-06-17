using System.Collections.ObjectModel;
using Utility.Observables.Generic;
using Utility.Trees.Abstractions;

namespace Utility.Trees
{
    public class DynamicTree : IDynamicTree, IObservable<ITree>, IObserver<ITree>
    {
        private new ObservableCollection<ITree> children => Current.Parent?.Items as ObservableCollection<ITree>;
        private ITree tree;
        private List<IObserver<ITree>> observers = new();
        private ITree current;
        private ITree toAdd;
        private State state = State.Current;
        private ObservableCollection<ITree> items;

        public DynamicTree(ITree root)
        {
            Current = tree = root;
            items = new(new[] { root });
        }

        public DynamicTree()
        {
            items = new();
        }

        public bool CanMoveUp => current?.Parent != null;
        public bool CanMoveDown => current?.HasItems == true || toAdd != null;
        public bool CanMoveForward => Index < children?.Count - 1;
        public bool CanMoveBack => Index > 0;
        public bool CanAdd => ToAdd != null;
        public bool CanRemove => current?.Parent != null;

        public ITree Up => Current.Parent;

        public ITree Down => current.HasItems ? Current[0] : toAdd;

        public ITree Forward => children[Index + 1];

        public ITree Back => children[Index - 1];

        public ITree Current
        {
            get => current ?? Tree as ITree ?? throw new Exception("g 3 ew33");
            set
            {
                if (tree == null)
                {
                    tree = value;
                    items.Add(tree);
                }

                if (current != value)
                {
                    Reset();
                    current = value;
                    State = State.Current;
                }
                else
                {
                }
            }
        }

        public ITree ToAdd => toAdd;

        public object Data { get => toAdd.Data; set => toAdd = new Tree(value) { Parent = Current, Key = Guid.NewGuid() }; }

        public IReadOnlyList<ITree> Items => items;

        public State State
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
            set
            {
                Reset();
                state = value;

                SetCurrent(value);

                Update();

                Broadcast();

                void SetCurrent(State value)
                {
                    switch (value)
                    {
                        case State.Forward:
                            if (CanMoveForward == false)
                                throw new Exception($"{State.Forward} V3 fds");
                            current = Forward;
                            break;

                        case State.Back:
                            if (CanMoveBack == false)
                                throw new Exception($"{State.Back} V3 fds");
                            current = Back;
                            break;

                        case State.Down:
                            if (CanMoveDown == false)
                                throw new Exception($"{State.Down} V3 fds");
                            if (current.HasItems == false)
                            {
                                current.Add(Down);
                            }
                            current = Down;
                            break;

                        case State.Up:
                            if (CanMoveUp == false)
                                throw new Exception($"{State.Up} V3 fds");
                            current = Up;
                            break;

                        case State.Add:
                            if (CanAdd == false)
                                throw new Exception($"{State.Add} V3 fds");
                            Add();
                            current.State = State.Add;
                            break;

                        case State.Remove:
                            if (CanRemove == false)
                                throw new Exception($"{State.Remove} V3 fds");
                            Remove();
                            current.State = State.Remove;
                            break;

                        case State.Current:
                            break;
                    }
                    void Add()
                    {
                        current.Add(ToAdd);
                        //current = ToAdd;
                    }

                    void Remove()
                    {
                        current.Parent.Remove(Current);
                        current = Current.Parent;
                    }
                }

                void Update()
                {
                    if (CanMoveUp)
                        Up.State = State.Up;
                    if (CanMoveDown)
                        Down.State = State.Down;
                    if (CanMoveForward)
                        Forward.State = State.Forward;
                    if (CanMoveBack)
                        Back.State = State.Back;

                    current.State = State.Current;
                }
            }
        }

        private void Reset()
        {
            if (CanMoveUp)
                Up.State = State.Default;
            if (CanMoveDown)
                Down.State = State.Default;
            if (CanMoveBack)
                Back.State = State.Default;
            if (CanMoveForward)
                Forward.State = State.Default;
        }

        public ITree Tree
        {
            get => tree; set
            {
                Current = tree = value;
                items.Clear();
                items.Add(tree);
                Broadcast();
            }
        }

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

        public IDisposable Subscribe(IObserver<ITree> observer)
        {
            return new Disposer<ITree>(observers, observer);
        }

        public ITree Last()
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

        public void OnNext(ITree value)
        {
            Current = value;
            Broadcast();
        }
    }
}