using Autofac;
using Autofac.Builder;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Utility.Tasks.DemoApp.Infrastructure
{
    public static class AutofacHelper
    {

        public static IRegistrationBuilder<TImplementer, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterDefault<TImplementer>(this IRegistrationBuilder<TImplementer, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder)
        {
            return builder.AsSelf().AsImplementedInterfaces().SingleInstance().OnActivated(Subscribe);
        }

        /// <summary>
        /// Solves circular references and better ensures all relevant observables are subscribed-to
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public static void Subscribe<T>(this IActivatedEventArgs<T> args)
        {
            var types = GetObserverTypes<T>();
            foreach (var type in types)
            {
                try
                {

                    Type[] typeArgs = { type };
                    Type generic = typeof(IObservable<>);
                    Type constructed = generic.MakeGenericType(typeArgs);

                    generic = typeof(IEnumerable<>);
                    var outerTypeArgs = new[] { constructed };
                    var enumerableConstruction = generic.MakeGenericType(outerTypeArgs);

                    var resolution = args.Context.Resolve(enumerableConstruction) as IEnumerable<IObservable<object>>;
                    var method = args.Instance.GetType().GetMethod(nameof(IObserver<T>.OnNext), typeArgs);
                    if (method == null)
                    {
                        throw new Exception("d33sfdg fdsf");
                    }

                    _ = resolution
                        .ToObservable()
                        .SelectMany(a => a)
                        .Where(a => types.Length == 1 || a.GetType() == type)
                        .Subscribe(a =>
                        {
                            try
                            {
                                method.Invoke(args.Instance, new[] { a });
                            }
                            catch (Exception ex)
                            {
                            }
                        });
                }
                catch (Exception ex)
                {

                }
            }


            static Type[] GetObserverTypes<T>()
            {
                return typeof(T).GetInterfaces()
                    .Where(ad => ad.Name == "IObserver`1")
                    .Select(a => a.GetGenericArguments().First())
                    .ToArray();
            }
        }
    }
}
