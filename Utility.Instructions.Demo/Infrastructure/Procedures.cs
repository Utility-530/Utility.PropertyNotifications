using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Utility.Instructions.Demo.Infrastructure;
using Utility.Trees;

namespace Utility.Instructions.Demo
{

    public record ProceduresState(int Index, ITree<Instruction> Current, ITree<Instruction> Forward, ITree<Instruction> Back, ITree<Instruction> Up, ITree<Instruction> CurrentBranch);



    public class Procedures : IObservable<ProceduresState>
    {
        const int resetCount = -1;
        private new ObservableCollection<ITree<Instruction>> children => currentBranch.Items as ObservableCollection<ITree<Instruction>>;


        private List<IObserver<ProceduresState>> observers = new();
        int count = resetCount;



        private ProceduresState State() => new(count, Current, Forward, Back, Up, currentBranch);

        Instruction root = new Instruction() { Value = "root" };

        public Procedures()
        {
            tree = new Tree<Instruction>(root) {  };
            currentBranch = Tree[root];
        }

        private Tree<Instruction> tree;
        private ITree<Instruction> currentBranch;

        public IEnumerable Children => children;

        public ITree<Instruction> Forward => children.Count <= count + 1 ? null : children[count + 1];

        public ITree<Instruction> Up => currentBranch.Parent;

        public ITree<Instruction> Back => count < 1 ? (count < 0 ? currentBranch.Parent : currentBranch) : children[count - 1];

        public ITree<Instruction> Previous => Past.TryPeek(out var guid) ? tree[guid] : null;

        public ITree<Instruction> Next => Future.TryPeek(out var guid) ? tree[ guid] : null;

        public ITree<Instruction> Current => count < 0 ? currentBranch : children.Count <= count ? null :  children[count];

        public ITree<Instruction> Tree => tree;

        public Stack<Guid> Past { get; set; } = new Stack<Guid>();
        public Stack<Guid> Future { get; set; } = new Stack<Guid>();
        public Dictionary<Guid, Stack<DateTime>> History { get; set; } = new Dictionary<Guid, Stack<DateTime>>();


        bool isDirty;


        public IEnumerable<Instruction> MoveUp()
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

        public Instruction MoveForward()
        {
            isDirty = true;

            if (count >= children.Count)
                return null;

            count++;

            SavePresentAsPast();
            foreach (var observer in observers)
                observer.OnNext(State());
            return Current.Data;
        }

        public Instruction MoveBack()
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


        public Instruction MoveNext()
        {
            Past.Push(Current.Key);
            var current = Future.Pop();
            var x = tree[current];
            count = x.Parent?.IndexOf(x) - 1 ?? -1;

            foreach (var observer in observers)
                observer.OnNext(State());
            return Next.Data;
        }

        public Instruction MovePrevious()
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

        public IDisposable Subscribe(IObserver<ProceduresState> observer)
        {
            return new Disposer<ProceduresState>(observers, observer);
        }

        public void Add(Instruction instruction)
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

        //public void Remove(Instruction instruction)
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



        public Instruction Last(InstructionType insertLast)
        {
            return children.LastOrDefault(a => (a.Data as Instruction).Type == insertLast).Data as Instruction;
        }


        public Instruction Last()
        {
            return children.LastOrDefault()?.Data as Instruction;
        }
    }
}
