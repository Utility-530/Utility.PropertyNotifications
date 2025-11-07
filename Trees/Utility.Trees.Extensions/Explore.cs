using System.Reactive.Disposables;
using Utility.Changes;
using Utility.Interfaces.NonGeneric;
using Utility.Reactives;
using Type = Utility.Changes.Type;

namespace Utility.Trees.Extensions
{
    public static class Explore
    {
        public static IDisposable With<T, TR>(T items, Func<T, TR, T> funcAdd, Action<T, TR> funcRemove, Action<T> funcClear, TR property, Predicate<TR>? predicate = default) where TR : IChildren
        {
            return With(items, funcAdd, funcRemove, funcClear, property, (Func<TR, IObservable<Changes.Set<TR>>>)(a => a.Children.AndChanges<TR>()), predicate ??= (TR a) => true);
        }

        public static IDisposable With<T, TR, TS>(TS item, Func<TS, T> funcItems, Func<T, TR, TS, TS> funcAdd, Action<T, TR> funcRemove, Action<T> funcClear, TR property, Predicate<TR>? predicate = default) where TR : IChildren
        {
            return With(item, funcItems, funcAdd, funcRemove, funcClear, property, a => a.Children.AndChanges<TR>(), predicate ??= (TR a) => true);
        }

        public static IDisposable With<T, TR>(T items, Func<T, TR, T> funcAdd, Action<T, TR> funcRemove, Action<T> funcClear, TR property, Func<TR, IObservable<Changes.Set<TR>>> func, Predicate<TR>? funcPredicate = null)
        {
            if (funcPredicate?.Invoke(property) == false)
                return Disposable.Empty;

            items = funcAdd(items, property);

            var disposable = func(property)
                .Subscribe(args =>
                {
                    foreach (var item in args)
                    {
                        if (item is Change { Type: Type.Add, Value: TR value })
                            _ = With(items, funcAdd, funcRemove, funcClear, value, func, funcPredicate);
                        else if (item is Change { Type: Type.Remove, Value: TR _value })
                            funcRemove(items, _value);
                        else if (item is Change { Type: Type.Reset })
                            funcClear(items);
                    }
                },
                e =>
                {
                },
                () =>
                {
                }
              );
            return disposable;
        }

        public static IDisposable With<T, TR, TS>(TS item, Func<TS, T> funcItems, Func<T, TR, TS, TS> funcAdd, Action<T, TR> funcRemove, Action<T> funcClear, TR property, Func<TR, IObservable<Changes.Set<TR>>> func, Predicate<TR>? funcPredicate = null)
        {
            if (funcPredicate?.Invoke(property) == false)
                return Disposable.Empty;

            var items = funcItems(item);
            item = funcAdd(items, property, item);

            var disposable = func(property)
                .Subscribe(args =>
                {
                    foreach (var _item in args)
                    {
                        if (_item is Change { Type: Type.Add, Value: TR value })
                            _ = With(item, funcItems, funcAdd, funcRemove, funcClear, value, func, funcPredicate);
                        else if (_item is Change { Type: Type.Remove, Value: TR _value })
                            funcRemove(funcItems(item), _value);
                        else if (_item is Change { Type: Type.Reset })
                            funcClear(funcItems(item));
                    }
                },
                e =>
                {
                },
                () =>
                {
                }
              );
            return disposable;
        }
    }
}