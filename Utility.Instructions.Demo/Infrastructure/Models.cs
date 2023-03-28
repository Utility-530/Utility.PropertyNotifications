using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Trees;
namespace Utility.Instructions.Demo
{


    public enum ChangeType
    {
        Insert, Remove, Update
    }

    public class Change<T, TKey> : Change<T>
        where T : class
        where TKey : class
    {
        public Change(T value, TKey parentKey, TKey key, int index, ChangeType type) : base(value, key, index, type)
        {
            ParentKey = parentKey;
        }

        public override TKey Key => base.Key as TKey;
        public TKey ParentKey { get; }

    }

    public class Change<T> : Change where T : class
    {
        public Change(T value, object key, int index, ChangeType type) : base(value, key, index, type)
        {
        }

        public override T Value => base.Value as T;
    }


    public class Change
    {

        public Change(object value, object key, int index, ChangeType type)
        {
            Value = value;
            Key = key;
            Index = index;
            Type = type;
        }

        public virtual object Key { get; }


        public virtual object Value { get; }
        public int Index { get; }
        public ChangeType Type { get; }
    }

    public record Key(Guid Guid);

    public interface IService : IObserver<Change<View, Key>>
    {
        IDisposable Subscribe(Guid key, IObserver<Change<View>> observer);

        bool HasItems(Guid key);
    }

    public class Service : IService
    {
        private readonly Guid root;

        //View tree;
        Dictionary<Guid, IObserver<Change<View>>> observers = new();

        public Service(Guid root)
        {
            Root = new View(this, "root", root) { };
            this.root = root;
        }

        public View Root { get; }

        public bool HasItems(Guid key)
        {
            if (TreeHelper2.Match(Root, a => a.Key.Equals(key) == true) is View view)
            {
                return view.Items.Count > 0;
            }
            return false;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Change<View, Key> change)
        {
            if (change.Value.Equals(Root))
                return;
            var x = TreeHelper2.Match(Root, a => a.Key.Equals(change.ParentKey.Guid) == true) as View;
            if (x is not null)
            {
                Modify(x);
            }
            else
            {
                Modify(Root);
            }

            //if (observers.TryGetValue(change.ParentKey.Guid, out var observer))
            //{
            //    if (TreeHelper2.Match(Root, a => a.Key.Equals(change.Key.Guid) == true) is null)
            //    {
            //        var view = change.Value.CloneTree() as View;
            //        view.IsExpanded = true;
            //        var cloneChange = new Change<View, Key>(view, change.ParentKey, change.Key, change.Index, change.Type);
            //        observer.OnNext(cloneChange);
            //    }
            //    else
            //    {
            //        observer.OnNext(change);
            //    }


            //}
            void Modify(View tree)
            {
                tree.OnNext(change);
            }
        }

        public IDisposable Subscribe(Guid key, IObserver<Change<View>> observer)
        {
            if (observers.ContainsKey(key))
                throw new Exception("t4 ssd");

            if (TreeHelper2.Match(Root, a => a.Key.Equals(key) == true) is ITree ot)
            {
                int i = 0;
                foreach (var x in ot.Items.ToArray())
                {
                    observer.OnNext(new Change<View, Key>(x.Data as View, new Key(ot.Key), new Key(x.Key), i, ChangeType.Insert));
                    i++;
                }
            }
            return new Utility.Observables.Disposer<Change<View>, Guid>(observers, new(root, observer));
        }
    }


}
