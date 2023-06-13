using Evan.Wpf;
using NetFabric.Hyperlinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Reactive;
using Utility.Helpers;
using Utility.WPF.Properties;
using Utility.WPF.Helpers;
using Utility.Reactive;

namespace Utility.WPF.Controls.Base
{
    public abstract class Controlx : Control, IPropertyListener, IControlListener, IDependencyObjectListener
    {
        private readonly NameTypeDictionary<SingleReplaySubject<object>> dict;

        IObservable<FrameworkElement> IControlListener.lazy { get; set; }
        Type IDependencyObjectListener.Type { get; } = typeof(Controlx);
        NameTypeDictionary<SingleReplaySubject<object>> IPropertyListener.dict => dict;

        public Controlx()
        {
            dict = new(this);

            this.LoadedChanges()
                .Take(1)
                .Subscribe(a =>
                {
                    (this as IPropertyListener).Init();
                });
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            (this as IPropertyListener).OnPropertyChanged(e);
            base.OnPropertyChanged(e);
        }
    }

    public interface IPropertyListener : IDependencyObjectListener
    {
        protected NameTypeDictionary<SingleReplaySubject<object>> dict { get; }

        public void Init([CallerMemberName] string? name = null)
        {
            //if (name != nameof(FrameworkElement.OnApplyTemplate))
            //{
            //    throw new Exception("Init must be called isnide OnApplyTemplate");
            //}

            var dependencyProperties = GetType().SelectDependencyProperties().TakeWhile(a => a.OwnerType.FullName.StartsWith("System") == false).ToArray();

            foreach (var dp in dependencyProperties)
            {
                if (dp.Name == "Id")
                {
                    dp.AddValueChanged(this.AsDependencyObject(), (sender, _) => dict[dp.Name].OnNext(this.AsDependencyObject().GetValue(dp)));
                }
            }

            foreach (var property in dependencyProperties)
            {
                dict[property.Name].OnNext(this.AsDependencyObject()?.GetValue(property));
            }

            //foreach (var (key, subject) in dict)
            //{
            //    var single = dependencyProperties.Where(a => a.Name == key).SingleOrDefault();
            //    if (single != null)
            //    {
            //        //if (single?.GetValue(this) is { } value)
            //        subject.OnNext((this.AsDependencyObject()?.GetValue(single)));
            //    }
            //}
        }

        public IObservable<T> Observable<T>(string? name = null)
        {
            return new Observable<T>(name == null ? dict[typeof(T)] : dict[name]);
        }

        public IObserver<T> Observer<T>(string? name = null)
        {
            return new Observer<T>(name == null ? dict[typeof(T)] : dict[name]);
        }

        public IObservable<object>? Observable(string name)
        {
            return dict[name];
        }

        public IObserver<object>? Observer(string name)
        {
            return dict[name];
        }

        //private void OnNext(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //{
        //    Init();
        //    string name = dependencyPropertyChangedEventArgs.Property.Name;
        //    object value = dependencyPropertyChangedEventArgs.NewValue;
        //    //await System.Threading.Tasks.Task.Run(() =>
        //    //{
        //    dict[name].OnNext(value);
        //    //});
        //}

        //public static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is IPropertyListener c)
        //        OnNext(e);
        //    else
        //        throw new Exception("b77adsf");

        //    void OnNext(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //    {
        //        string name = dependencyPropertyChangedEventArgs.Property.Name;
        //        object value = dependencyPropertyChangedEventArgs.NewValue;
        //        //await System.Threading.Tasks.Task.Run(() =>
        //        //{
        //        c.dict[name].OnNext(value);
        //        //});
        //    }

        //}

        public void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            dict[e.Property.Name].OnNext(e.NewValue);

            //  base.OnPropertyChanged(e);
        }
    }

    public interface IControlListener
    {
        protected IObservable<FrameworkElement> lazy { get; set; }

        public IObservable<T> Control<T>(string? name = null) where T : FrameworkElement
        {
            return Observable.Create<T>(observer =>
            {
                lazy ??= SelectFramworkElements();
                var dis = (name == null ?
                 lazy.OfType<T>() :
                 lazy.OfType<T>().Where(a => a.Name == name))
                 .Subscribe(a =>
                 {
                     observer.OnNext(a);
                 });
                //observer.OnCompleted();
                return dis;
            })
            .ObserveOnDispatcher()
            .SubscribeOnDispatcher();
        }

        protected IObservable<FrameworkElement> SelectFramworkElements()
        {
            var dependencyObject = this as DependencyObject ?? throw new Exception("Expected type is DependencyObject");
            if (dependencyObject is FrameworkElement control)
            {
                if (control.IsLoaded == false)
                {
                    return Observable.Create<FrameworkElement[]>(observer =>
                    {
                        return control
                            .LoadedChanges()
                            .Subscribe(a =>
                            {                         
                                var t = dependencyObject
                                .FindChildren<FrameworkElement>()
                                .ToArray();
                                observer.OnNext(t);
                            });
                    })
                         .ObserveOnDispatcher()
                         .SubscribeOnDispatcher()
                         .ToReplaySubject(1)
                         .SelectMany();
                }
            }
            return Observable
                .Return(dependencyObject
                .FindVisualChildren<FrameworkElement>()
                .ToArray())
                .SelectMany(); 
        }
    }

    internal class Observer<T> : IObserver<T>
    {
        private readonly ISubject<object> subject;

        public Observer(ISubject<object> subject)
        {
            this.subject = subject;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException("fsdd7777777f");
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException("fsdxxxd7777777f");
        }

        public void OnNext(T value)
        {
            subject.OnNext(value);
        }
    }

    internal class Observable<T> : IObservable<T>
    {
        private readonly ISubject<object> subject;

        public Observable(ISubject<object> subject)
        {
            this.subject = subject;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Cast<T>().Subscribe(observer);
        }
    }

    public class SingleReplaySubject<T> : ISubject<T>
    {
        private readonly ReplaySubject<T> subject;

        public SingleReplaySubject()
        {
            subject = new ReplaySubject<T>(1);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            subject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Subscribe(observer);
        }
    }

    public static class NameTypeDictionaryHelper
    {
        public static DependencyObject AsDependencyObject(this IDependencyObjectListener listener)
        {
            return listener as DependencyObject ?? throw new Exception("Expected type to be " + nameof(DependencyObject));
        }

        public static IObservable<T> Observable<T>(this IPropertyListener listener, string? name = null)
        {
            return listener.Observable<T>(name);
        }

        public static IObserver<T> Observer<T>(this IPropertyListener listener, string? name = null)
        {
            return listener.Observer<T>(name);
        }

        public static IObservable<T> Observer<T>(this IControlListener listener, string? name = null)
            where T : FrameworkElement
        {
            return listener.Control<T>(name);
        }

        public static IObservable<object> Observable(this IPropertyListener listener, string name)
        {
            return listener.Observable(name);
        }

        public static IObserver<object> Observer(this IPropertyListener listener, string name)
        {
            return listener.Observer(name);
        }

        public static IObservable<T> Control<T>(this IControlListener listener, string? name = null)
            where T : FrameworkElement
        {
            return listener.Control<T>(name);
        }

        //        public static IObservable<object> ObservableOfName<TObservable>(this NameTypeDictionary<TObservable> dict, string name)
        //            where TObservable : IObservable<object>, new()
        //        {
        //            return dict[name];
        //        }

        //        public static IObservable<object> ObservableOfType<TObservable>(this NameTypeDictionary<TObservable> dict, Type type)
        //              where TObservable : IObservable<object>, new()
        //        {
        //            return dict[type];
        //        }

        //        public static IObserver<object> ObserverOfName<TObservable>(this NameTypeDictionary<TObservable> dict, string name )
        //              where TObservable : IObserver<object>, new()
        //        {
        //            return  dict[name];
        //        }

        //        public static IObserver<object> Observer<TObservable>(this NameTypeDictionary<TObservable> dict, Type type)
        //      where TObservable : IObserver<object>, new()
        //{
        //            return dict[type];
        //        }

        /// <summary>
        /// Whatches for any changes to dependency properties
        /// </summary>
        /// <returns></returns>
        //public static IObservable<Dictionary<string, object>> Any<T>(this NameTypeDictionary<T> dict)
        //      where T : IObservable<object>, new()
        //{
        //    return Observable.Create<Dictionary<string, IObservable<object>>>(observer =>
        //    {
        //        //Dictionary<string, IObservable<object>> dict = new();
        //        Subject<Dictionary<string, object>> sub = new();
        //        List<IDisposable> xx = new();
        //        foreach (var (key, value) in dict)
        //        {
        //            xx.Add(value.Subscribe(a => { dict[key] = a; sub.OnNext(dict); }));
        //        }
        //        return new System.Reactive.Disposables.CompositeDisposable(xx);
        //    });
        //}
    }


    public class DependencyPropertyFactory<TControl> where TControl : DependencyObject
    {
        public static DependencyProperty Register<TProperty>(string? name = null, TProperty? initialValue = default, CoerceValueCallback? callBack = null)
        {
            return DependencyProperty.Register(name ??= GetName(typeof(TProperty), name), typeof(TProperty), typeof(TControl), GetPropertyMetadata(name, initialValue, callBack));

            static PropertyMetadata GetPropertyMetadata(string name, TProperty? initialValue, CoerceValueCallback? callBack)
            {
                return typeof(TControl).IsCastableTo(typeof(IPropertyListener)) ?
                      MetaDataFactory<TControl>.Create(a => (a as IPropertyListener)?.Observer<TProperty>(name) ?? throw new Exception("XcxcX"), initialValue, callBack) :
                      new PropertyMetadata(initialValue, (a, b) => { }, coerceValueCallback: callBack);
            }
        }

        public static DependencyProperty Register(string name, CoerceValueCallback? callBack = null)
        {
            return DependencyProperty.Register(name, GetProperty(name).PropertyType, typeof(TControl), GetPropertyMetadata(name, callBack));

            static PropertyMetadata GetPropertyMetadata(string name, CoerceValueCallback? callBack)
            {
                return typeof(TControl).IsCastableTo(typeof(IPropertyListener)) ?
                      MetaDataFactory<TControl>.Create(a => (a as IPropertyListener)?.Observer(name) ?? throw new Exception("XcxcX"), callBack) :
                      new PropertyMetadata(default, (a, b) => { }, coerceValueCallback: callBack);
            }
        }

        public static DependencyProperty Register(string name, Type propertyType, CoerceValueCallback? callBack = null)
        {
            return DependencyProperty.Register(name, propertyType, typeof(TControl), GetPropertyMetadata(name, callBack));

            static PropertyMetadata GetPropertyMetadata(string name, CoerceValueCallback? callBack)
            {
                return typeof(TControl).IsCastableTo(typeof(IPropertyListener)) ?
                      MetaDataFactory<TControl>.Create(a => (a as IPropertyListener)?.Observer(name) ?? throw new Exception("XcxcX"), callBack) :
                      new PropertyMetadata(default, (a, b) => { }, coerceValueCallback: callBack);
            }
        }

        private static string GetName(Type propertyType, string? name = null)
        {
            return name ?? GetProperty(propertyType).Name;
        }

        private static PropertyInfo GetProperty(Type propertyType)
        {
            var props = typeof(TControl).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                 .Where(a => a != null)
                                                 .Where(p => p.PropertyType == propertyType && p.DeclaringType?.FullName?.StartsWith("System") == false)
                                                 .ToArray();

            if (props.Length > 1)
            {
                throw new Exception("Number of matching properties exceeds one. Use name to make search more specific.");
            }
            if (props.Length == 0)
            {
                throw new Exception($"Number of matching properties is 0. Specify a name for property {propertyType.Name}");
            }
            return props.Single();
        }

        private static PropertyInfo GetProperty(string name)
        {
            var props = typeof(TControl).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                        .Where(a => a != null)
                                        .Where(p => p.Name == name && p.DeclaringType?.FullName?.StartsWith("System") == false)
                                        .ToArray();

            if (props.Length > 1)
            {
                throw new Exception("Number of matching properties exceeds one. Use name to make search more specific.");
            }
            if (props.Length == 0)
            {
                throw new Exception($"Number of matching properties is 0. Specify a name for property {name}");
            }
            return props.Single();
        }

        public static DependencyProperty Register<TProperty>(string name, Func<TControl, IObserver<TProperty>> observer, TProperty? initialValue = default) =>
            DependencyProperty.Register(name, typeof(TProperty), typeof(TControl), MetaDataFactory<TControl>.Create(observer, initialValue));
    }

    internal class MetaDataFactory<TControl> where TControl : DependencyObject
    {
        public static PropertyMetadata Create<TProperty>(string? name = null, TProperty? initialValue = default, CoerceValueCallback? callBack = null) =>
         typeof(TControl).IsCastableTo(typeof(IPropertyListener)) ?
            MetaDataFactory<TControl>.Create(a => (a as IPropertyListener)?.Observer<TProperty>(name) ?? throw new Exception("XX"), initialValue, callBack) :
           new PropertyMetadata(initialValue);

        public static PropertyMetadata Create<TProperty>(Func<TControl, IObserver<TProperty>> observer, TProperty? value = default, CoerceValueCallback? callBack = null) =>
            new(value, PropertyChangedCallbackFactory.Create(observer!, value), callBack);

        private class PropertyChangedCallbackFactory
        {
            public static PropertyChangedCallback Create<T>(Func<TControl, IObserver<T>> observer, T initialValue) =>
new((d, e) => new DependencyPropertyChangedObserver<TControl, T>(observer, initialValue).OnNext(d, e));
        }
    }

    internal class DependencyPropertyChangedObserver<TControl, T> where TControl : DependencyObject
    {
        private readonly Func<TControl, IObserver<T>> observer;

        public DependencyPropertyChangedObserver(Func<TControl, IObserver<T>> observer, T? initialValue)
        {
            this.observer = observer;
        }

        public void OnNext(DependencyObject d, DependencyPropertyChangedEventArgs e) => observer(d as TControl).OnNext((T)e.NewValue);
    }

    public static class DependencyObjectHelper
    {
        public static void Convert<T, TS, TR>(this DependencyObject a, DependencyPropertyChangedEventArgs e, Func<T, TS> map)
            where T : DependencyObject where TS : IObserver<TR>
            => map(a as T).OnNext((TR)e.NewValue);
    }

    public interface IDependencyObjectListener
    {
        protected Type Type { get; }
    }

    public interface INameTypeDictionary
    { }

    public class NameTypeDictionary<TValue> : INameTypeDictionary, IEnumerable<KeyValuePair<string, TValue>> where TValue : new()
    {
        private readonly Dictionary<string, TValue> subjects = new();
        private readonly Dictionary<Type, string> nameTypeDictionary = new();
        private readonly Lazy<DependencyProperty[]> propertyInfos;

        public NameTypeDictionary(object @object)
        {
            this.propertyInfos = new(() => @object.GetType().SelectDependencyProperties().ToArray());
        }

        public TValue this[string name]
        {
            get => GetValue(name);
        }

        public TValue this[Type type]
        {
            get => GetValue(type);
        }

        public TValue GetValue(Type type)
        {
            return GetValue(GetName(type));
        }

        private string GetName(Type type)
        {
            if (nameTypeDictionary.ContainsKey(type))
            {
                return nameTypeDictionary[type];
            }
            else
            {
                var where = propertyInfos.Value.Where(a => a.PropertyType == type).ToArray();
                if (where.Any())
                    if (where.Length == 1)
                    {
                        if (nameTypeDictionary.ContainsKey(type))
                            throw new Exception($"dictionary already contains type, {type.Name}!");
                        return nameTypeDictionary[type] = where.Single().Name;
                    }
                    else
                        throw new Exception("UnExpected multiple types");
                else
                    throw new Exception("No types match");
            }
        }

        private TValue GetValue(string? name = null)
        {
            if (name == null) throw new ArgumentException("666jj");

            if (subjects.ContainsKey(name))
                return subjects[name];

            var where = propertyInfos.Value.Where(a => a.Name == name).ToArray();
            if (where.Any())
                if (where.Length == 1)
                {
                }
                else if (where.Length > 1)
                    throw new Exception("UnExpected multiple types");

            return subjects[name] = new();
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return subjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return subjects.GetEnumerator();
        }

        //public KeyValuePair<string, TValue> Current => Enumerator.Current;

        //object IEnumerator.Current => Enumerator.Current;

        //public void Dispose()
        //{
        //    Enumerator.Dispose();
        //    enumerator = null;
        //}
        //public bool MoveNext()
        //{
        //    return Enumerator.MoveNext();
        //}

        //public void Reset()
        //{
        //    Enumerator.Reset();
        //}
    }
}