using Utility.PropertyTrees.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Enums;
using Utility.Models;
using Utility.Observables;
using Utility.Infrastructure;
using System.Collections;
using Utility.Observables.NonGeneric;

namespace Utility.PropertyTrees.Infrastructure
{


    public class PropertyStore : IPropertyStore
    {
        private readonly Dictionary<IEquatable, IObserver> dictionary = new();

        //readonly Repository repo;
        //private readonly History history = new();

        //private readonly Controllable controllable = new();
        //private DispatcherTimer timer = new();

        private Lazy<IRepository> repository = new(() =>
        {
            var directory = Directory.CreateDirectory("../../../Data");
            return new SqliteRepository(directory.FullName);
        });

        public PropertyStore()
        {
            //controllable.Subscribe(this);
            //history.Subscribe(this);
            //timer.Subscribe(a =>
            //{
            //    if (history.Future.GetEnumerator().MoveNext())
            //        history.Forward();
            //});
        }

        protected virtual IRepository Repository
        {
            get => repository.Value;
        }

        public IEnumerable<IObserver> Observers => dictionary.Values;

        //public IHistory History => history;
        //public IControllable Controllable => controllable;

        //public void GetValue(IEquatable key)
        //{
        //    if (key is not Key { } _key)
        //    {
        //        throw new Exception("reg 43cs ");
        //    }

        //    Observable
        //        .Return(new Order { Key = _key, Access = Access.Get })
        //        .Subscribe(history.OnNext);
        //}

        //public void SetValue(IEquatable key, object value)
        //{
        //    if (key is not Key { } _key)
        //    {
        //        throw new Exception("reg 43cs ");
        //    }
        //    Observable
        //        .Return(new Order { Key = _key, Access = Access.Set, Value = value })
        //        .Subscribe(history.OnNext);
        //}

        public IDisposable Subscribe(IObserver observer)
        {
            dictionary.Add(observer, observer);
            return new Disposer<IEquatable>(dictionary, observer, observer);
        }

        //public string Validate(string memberName)
        //{
        //    return string.Empty;
        //}

        // Move this into history
        //public async Task<Guid> GetGuidByParent(IEquatable key)
        //{
        //    var childKey = await Repository.FindKeyByParent(key);
        //    return (childKey as Key)?.Guid ?? throw new Exception("dfb 43 4df");
        //}

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public async void OnNext(object value)
        {
            if (value is not Order order)
            {
                throw new Exception("g 3434 3");
            }

            order.Progress = 0;
            switch (order.Access)
            {
                case Access.Get:
                    {
                        try
                        {
                            var guid = await Repository.FindKeyByParent(order.Key);
                            order.Progress = 50;
                            var find = await Repository.FindValue(guid);
                            order.Progress = 100;

                            if (find != null)
                            {
                                Update(find, order);
                            }
                        }
                        catch (Exception ex)
                        {
                            order.Exception = ex;
                        }

                        break;
                    }
                case Access.Set:
                    {
                        try
                        {
                            var guid = await Repository.FindKeyByParent(order.Key);
                            order.Progress = 50;
                            var find = await Repository.FindValue(guid);
                            await Repository.UpdateValue(guid, order.Value);
                            order.Progress = 100;
                            Update(find, order);
                        }
                        catch (Exception ex)
                        {
                            order.Exception = ex;
                        }

                        break;
                    }
            }
        }

        private void Update(object newValue, Order order)
        {
            if (dictionary.TryGetValue(order.Key, out var observer))
            {
                observer.OnNext(new PropertyChange(order.Key, newValue, order.Value));
            }
        }

        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        //public static PropertyStore Instance { get; } = new();

        private class KeyComparer : IEqualityComparer<IEquatable>
        {
            public bool Equals(IEquatable? x, IEquatable? y)
            {
                return x.Equals(y);
            }

            public int GetHashCode([DisallowNull] IEquatable obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}