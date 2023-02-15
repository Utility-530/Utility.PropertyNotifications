using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Utility.Trees
{



    public class DynamicTree<T> : IObservable<TreeState>, IObserver<TreeState>
    {
        private new ObservableCollection<ITree<T>> children => Current.Parent?.Items as ObservableCollection<ITree<T>>;
        private Tree<T> tree;
        //private ITree<T> currentBranch;

        private List<IObserver<TreeState>> observers = new();
        //private int index = resetCount;
        private ITree<T> toRemove;
        private ITree<T> current;

        private TreeState TreeState() => new(
            Index,
            State,
            Current,
            CanMoveForward ? Forward : default,
            CanMoveBack ? Back : default,
            CanMoveUp ? Up : default,
            CanMoveDown ? Down : default,
            CanAdd ? ToAdd : default,
            CanRemove ? ToRemove : default);


        public DynamicTree(T root)
        {
            tree = new Tree<T>(root) { };
            Current = Tree[root];
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

        //public ITree<T> Previous => Past.TryPeek(out var guid) ? tree[guid] : null;

        //public ITree<T> Next => Future.TryPeek(out var guid) ? tree[guid] : null;

        public ITree<T> Current
        {
            get
            {
                return current;
            }
            private set
            {
                current = value;
                //index = value.Parent.IndexOf(value);
            }
        }


        public ITree<T> ToRemove => toRemove;

        public ITree<T> ToAdd => toAdd;

        private ITree<T> toAdd;
        public T Data { get=> toAdd.Data; set=> toAdd = new Tree<T>(value) { Parent = Current }; }

        public State State { get; set; }

        public ITree<T> Tree => tree;

        public int Index => Current.Parent?.IndexOf(Current) ?? 0;

        public void MoveUp()
        {
            Current = Up;

            State = State.Up;

            Broadcast();

        }

        public void MoveForward()
        {
            Current = Forward;
            State = State.Forward;

            Broadcast();
                }

        public void MoveDown()
        {
            Current = Down;
            State = State.Down;

            Broadcast();
        }

        public void MoveBack()
        {
            Current = Back;
            State = State.Back;
            Broadcast();
        }


        public void Dispose()
        {
            children.Clear();
        }

        public void Add()
        {
            toRemove = ToAdd;
            Current.Add(toRemove);
            Current = toRemove;
            State = State.Add;
            toAdd = null;
            Broadcast();
        }

        public void Remove()
        {
            var parent = Current.Parent;
            Current.Parent.Remove(Current);
            Current = parent;
            State = State.Remove;
            Broadcast();
        }

        public void Broadcast()
        {
            foreach (var observer in observers)
                observer.OnNext(TreeState());
        }

        public IDisposable Subscribe(IObserver<TreeState> observer)
        {
            return new Disposer<TreeState>(observers, observer);
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

        public void OnNext(TreeState value)
        {
            //index = value.Index;
            State = value.State;
            Current = value.Current as ITree<T>;
            Data = (value.Add as ITree<T>).Data;
            toRemove = value.Remove as ITree<T>;
        }

        public void OnPrevious(TreeState data)
        {
            throw new NotImplementedException();
        }
    }
}
