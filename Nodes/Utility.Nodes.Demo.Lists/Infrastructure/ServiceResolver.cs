using Splat;
using System.Reflection;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Services
{

    public class MethodNode : NotifyPropertyClass
    {

        Dictionary<string, object> names = new();
        private object outValue;
        private readonly Lazy<Dictionary<string, ValueModel<object>>> inValues;

        public MethodNode(Method methodInfo)
        {
            var parameters = methodInfo.MethodInfo.GetParameters();
            inValues = new(() => parameters.ToDictionary(a => a.Name ?? throw new Exception("s e!"), a =>
            {
                var model = new ValueModel<object> { Name = a.Name };
                model.WhenReceivedFrom(a => a.Value, includeNulls: false)
                .Subscribe(da =>
                {
                    names[a.Name] = da;
                    if (names.Count == parameters.Length)
                    {
                        var result = methodInfo.Execute(names);
                        OutValue = result;
                    }
                });
                return model;
            }));
        }

        public Dictionary<string, ValueModel<object>> InValues => inValues.Value;

        public object this[string index]
        {
            set => InValues[index].Value = value;
        }

        public object OutValue
        {
            get => outValue;
            set => RaisePropertyChanged(ref outValue, value);
        }

        public MethodInfo Info { get; set; }
    }


    public class ServiceResolver
    {
        Dictionary<Type, IMethodParameter> List { get; } = new();
        Dictionary<Method, MethodNode> Cache { get; } = new();

        public void Observe<TParam>(IValueModel tModel)
            where TParam : IMethodParameter
        {
            tModel.WhenReceivedFrom(a => (a as IValue).Value, includeNulls: false)
                .Subscribe(value =>
                {
                    if (get<TParam>() is IMethodParameter methodParameter)
                    {
                        if (get(methodParameter) is MethodNode cache)
                        {
                            cache.InValues[methodParameter.Name].Value = value;
                        }
                    }
                });

            tModel.WithChangesTo(a => (a as IValue).Value, includeNulls: false)
                .Subscribe(value =>
                {
                    if (get<TParam>() is IMethodParameter methodParameter)
                    {
                        if (get(methodParameter) is MethodNode cache)
                        {
                            cache.InValues[methodParameter.Name].Value = value;
                        }
                    }
                });
        }

        public void ReactTo<TParam>(IValueModel tModel, Func<object, object>? transformation = null, Action<object>? setAction = null)
            where TParam : IMethodParameter
        {
            setAction ??= (a) => (tModel as ISetValue).Value = a;
            if (get<TParam>() is IMethodParameter methodParameter)
            {
                if (get(methodParameter) is MethodNode cache)
                {
                    cache.WithChangesTo(a => a.OutValue).Subscribe(a => setAction(transformation != null ? transformation.Invoke(a) : a));
                }
            }

        }

        public void Connect<TParam, TParam2>()
            where TParam : IMethodParameter
            where TParam2 : IMethodParameter
        {
            if (get<TParam>() is IMethodParameter methodParameter)
            {
                if (get<TParam2>() is IMethodParameter methodParameter2)
                {
                    if (get(methodParameter) is MethodNode cache)
                    {
                        if (get(methodParameter) is MethodNode cache2)
                        {
                            cache.WithChangesTo(a => a.OutValue)
                                .Subscribe(a => cache2.InValues[methodParameter2.Name].Value = a);

                        }

                    }
                }
            }

        }

        private IMethodParameter get<TService>()
        {
            return List.Get(typeof(TService), a => (IMethodParameter)Activator.CreateInstance(a));
        }
        private MethodNode get(IMethodParameter methodParameter)
        {
            return Cache.Get(methodParameter.Method, a => new MethodNode(a) { });
        }
    }

    public static class ServiceHelper
    {
        public static IValueModel ToValueModel<T>(this IObservable<T> observable)
        {
            var valueModel = new ValueModel<T>() { Name = typeof(T).Name };
            observable.Subscribe(a => valueModel.Value = a);
            return valueModel;
        }

        public static IObservable<T> Observe<TParam, T>(this IObservable<T> observable) where TParam : IMethodParameter
        {
            Observe<TParam>(observable.ToValueModel());
            return observable;
        }

        public static void Observe<TParam>(this IValueModel tModel) where TParam:IMethodParameter => 
            Locator.Current.GetService<ServiceResolver>().Observe<TParam>(tModel);

        public static void ReactTo<TParam>(this IValueModel tModel,Func<object, object>? transformation = null, Action<object>? setAction = null) where TParam:IMethodParameter => 
            Locator.Current.GetService<ServiceResolver>().ReactTo<TParam>(tModel);



    }
}
