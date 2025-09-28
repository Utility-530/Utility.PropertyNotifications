using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;

namespace Utility.Trees
{
    public class Branch : IClone
    {
        static int number = 0;

        public object Clone()
        {
            return new Branch { Number = number++ };
        }

        public int Number { get; set; }

        public override string ToString()
        {
            return nameof(Branch) + " " + Number;
        }
    }
    public class Root : Branch
    {
        public override string ToString()
        {
            return nameof(Root) + " " + Number;
        }
    }

    public class DynamicTree : IDynamicTree, System.IObservable<ITree>, System.IObserver<ITree>, INotifyPropertyChanged
    {
        private IReadOnlyList<ITree> children => (Current as IGetParent<IReadOnlyTree>).Parent?.Children.Cast<ITree>().ToList();
        private ITree tree;
        private List<System.IObserver<ITree>> observers = new();
        private ITree current;
        private ObservableCollection<ITree> items;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DynamicTree(ITree root)
        {
            Current = tree = root;
            items = new(new[] { root });
        }

        public DynamicTree() 
        {
            items = new(Array.Empty<ITree>());
        }

        public bool CanMoveUp => (current as IGetParent<IReadOnlyTree>)?.Parent != null;
        public bool CanMoveDown => current?.HasChildren == true;
        public bool CanMoveForward => Index < children?.Count - 1;
        public bool CanMoveBack => Index > 0;
        public bool CanAdd => current != null;
        public bool CanRemove => (current as IGetParent<IReadOnlyTree>)?.Parent != null;

        public ITree Up => (Current as IGetParent<IReadOnlyTree>).Parent as ITree;

        public ITree Down => Current[0];

        public ITree Forward => children[Index + 1];

        public ITree Back => children[Index - 1];

        //Task<ITree> Add => Current.Add();

        //Task<ITree> Remove => Current.Remove();



        public ITree Current
        {
            get => current ?? Tree ?? throw new Exception("g 3 ew33");
            set
            {
                if (tree == null)
                {
                    tree = value;
                    items.Add(tree);
                }

                if (current != value)
                {
                    //Reset();
                    current = value;
                    //state = State.Current;
                }
                else
                {
                }

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
            }
        }

        //public ITree ToAdd => toAdd;

        //public object Data { get => toAdd.Data; set => toAdd = new Tree(value) { Key = Guid.NewGuid() }; }

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
                //Reset();
                //state = value;                
                SetCurrent(value);
                //Update();
                Broadcast();

                async void SetCurrent(State value)
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
                            //if (current.HasItems == false)
                            //{
                            //    current.Add(Down);
                            //}
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
                            //Add();
                            //current = await Add;
                            //current.State = State.Add;
                            break;

                        case State.Remove:
                            if (CanRemove == false)
                                throw new Exception($"{State.Remove} V3 fds");
                            //current = await Remove;
                            //current.State = State.Remove;
                            break;

                        case State.Current:
                            break;

                    }
                }

                //void Update()
                //{
                //    if (CanMoveUp)
                //        Up.State = State.Up;
                //    if (CanMoveDown)
                //        Down.State = State.Down;
                //    if (CanMoveForward)
                //        Forward.State = State.Forward;
                //    if (CanMoveBack)
                //        Back.State = State.Back;

                //    current.State = State.Current;
                //}
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
            }
        }

        //private void Reset()
        //{
        //    if (CanMoveUp)
        //        Up.State = State.Default;
        //    if (CanMoveDown)
        //        Down.State = State.Default;
        //    if (CanMoveBack)
        //        Back.State = State.Default;
        //    if (CanMoveForward)
        //        Forward.State = State.Default;
        //}

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

        public int Index => (Current as IGetParent<IReadOnlyTree>).Parent?.IndexOf(Current) ?? 0;

        public void Dispose()
        {
            ((Current as IGetParent<IReadOnlyTree>).Parent?.Children as IList)?.Clear();
        }

        public void Broadcast()
        {
            foreach (var observer in observers)
                observer.OnNext(Current);
        }

        public IDisposable Subscribe(System.IObserver<ITree> observer)
        {

            return new Utility.Observables.Disposer<ITree>(observers, observer);
        }



        //public ITree Last()
        //{
        //    return children.LastOrDefault();
        //}

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

        public IDisposable Subscribe(System.IObserver<State> observer)
        {
            throw new NotImplementedException();
        }
    }
}