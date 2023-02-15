using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Utility.Instructions.Demo.Infrastructure;
using Utility.Trees;

namespace Utility.Instructions.Demo
{

    public record TreeState(int Index, ITree Current, ITree Forward, ITree Back, ITree Up, ITree CurrentBranch);
 


    public class DynamicTree<T> : IObservable<TreeState> where T : new()
    {
        const int resetCount = -1;
        private new ObservableCollection<ITree<T>> children => currentBranch.Items as ObservableCollection<ITree<T>>;


        private List<IObserver<TreeState>> observers = new();
        int count = resetCount;



        private TreeState State() => new(count, Current, Forward, Back, Up, currentBranch);

        //T root = new T() { Value = "root" };

        public DynamicTree(T root)
        {
            tree = new Tree<T>(root) { };
            currentBranch = Tree[root];
        }

        private Tree<T> tree;
        private ITree<T> currentBranch;

        public IEnumerable Children => children;

        public ITree<T> Forward => children.Count <= count + 1 ? null : children[count + 1];

        public ITree<T> Up => currentBranch.Parent;

        public ITree<T> Back => count < 1 ? (count < 0 ? currentBranch.Parent : currentBranch) : children[count - 1];

        public ITree<T> Previous => Past.TryPeek(out var guid) ? tree[guid] : null;

        public ITree<T> Next => Future.TryPeek(out var guid) ? tree[guid] : null;

        public ITree<T> Current => count < 0 ? currentBranch : children.Count <= count ? null : children[count];

        public ITree<T> Tree => tree;

        public Stack<Guid> Past { get; set; } = new Stack<Guid>();
        public Stack<Guid> Future { get; set; } = new Stack<Guid>();
        public Dictionary<Guid, Stack<DateTime>> History { get; set; } = new Dictionary<Guid, Stack<DateTime>>();


        bool isDirty;


        public IEnumerable<T> MoveUp()
        {
            isDirty = true;
            if (count >= children.Count)
                yield break;
            if (count < 0)
            {
                currentBranch = currentBranch.Parent;
                if (currentBranch == null)
                    yield break;
                count = currentBranch.Parent.IndexOf(currentBranch);
            }

            SavePresentAsPast();

            foreach (var x in currentBranch.GetChildren(false))
                yield return x.Data;

            foreach (var observer in observers)
                observer.OnNext(State());
        }

        public T MoveForward()
        {
            isDirty = true;

            if (count >= children.Count)
                return default;

            count++;

            SavePresentAsPast();
            foreach (var observer in observers)
                observer.OnNext(State());
            return Current.Data;
        }

        public T MoveBack()
        {
            if (count < 0)
            {
                if (count < -1)
                    throw new Exception("g 4333");

                currentBranch = currentBranch.Parent;
                Reset();
            }
            else
            {
                count--;
            }

            isDirty = true;
            SavePresentAsPast();

            foreach (var observer in observers)
                observer.OnNext(State());
            return Forward.Data;
        }


        public T MoveNext()
        {
            Past.Push(Current.Key);
            var current = Future.Pop();
            var x = tree[current];
            count = x.Parent?.IndexOf(x) - 1 ?? -1;

            foreach (var observer in observers)
                observer.OnNext(State());
            return Next.Data;
        }

        public T MovePrevious()
        {
            var current = Past.Pop();
            Future.Push(current);
            var x = tree[current];
            count = x.Parent?.IndexOf(x) - 1 ?? -1;

            foreach (var observer in observers)
                observer.OnNext(State());
            return Previous.Data;
        }

        public void Reset()
        {
            count = resetCount;
            isDirty = false;
        }

        public void Dispose()
        {
            children.Clear();
        }

        public void RemoveFuture()
        {
            while (children.Count > count + 2)
                children.Remove(children[^1]);


        }

        public IDisposable Subscribe(IObserver<TreeState> observer)
        {
            return new Disposer<TreeState>(observers, observer);
        }

        public void Add(T instruction)
        {
            if (isDirty)
            {
                RemoveFuture();
                isDirty = false;
            }
            if (count != -1)
            {
                currentBranch = currentBranch[count];
                Reset();
            }
            currentBranch.Add(instruction);
            foreach (var observer in observers)
                observer.OnNext(State());
        }

        //public void Remove(T instruction)
        //{
        //    if (isDirty)
        //    {
        //        RemoveFuture();
        //        isDirty = false;
        //    }
        //    if (count < 0)
        //    {
        //        currentBranch = currentBranch.Parent;
        //        count = currentBranch.Parent.Items.IndexOf(currentBranch);
        //    }

        //    currentBranch.Items.Remove(instruction);
        //    foreach (var observer in observers)
        //        observer.OnNext(State());
        //}

        public void SavePresentAsPast()
        {
            Past.Push(Current.Key);
            (History.TryGetValue(Current.Key, out Stack<DateTime> value) ? value : History[Current.Key] = new Stack<DateTime>()).Push(DateTime.Now);
        }

        //public void SavePresentAsFuture()
        //{
        //    Future.Push(Current.Key);
        //    History[Current.Key] = (History.ContainsKey(Current.Key) ? History[Current.Key] : new Stack<DateTime>()).Push(DateTime.Now);
        //}



        //public T Last(InstructionType insertLast)
        //{
        //    return children.LastOrDefault(a => (a.Data as T).Type == insertLast).Data as T;
        //}


        public ITree<T> Last()
        {
            return children.LastOrDefault();
        }
    }
}
