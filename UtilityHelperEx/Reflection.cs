using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using UtilityHelper;

namespace UtilityHelperEx
{
    public static class ReflectionHelper
    {
        public static IObservable<Assembly> SelectAssemblies()
        {
            return Observable.Create<Assembly>(obs =>
            {
                var dis1 = ReflectionHelper.LoadedAssemblies().Subscribe(obs);
                var dis2 = Task.Run(() => UtilityHelper.ReflectionHelper.GetAssemblies())
                .ToObservable()
                .Subscribe(asses =>
                {
                    foreach (var ass in asses)
                    {
                        obs.OnNext(ass);
                    }
                });
                return new CompositeDisposable(dis1, dis2);
            });
        }

        public static IEnumerable<(string, Func<T?>)> GetStaticMethods<T>(this Type t, params object[] parameters)
        {
            var typename = typeof(T).Name;
            return t
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                       // filter by return type
                       .Where(a => a.ReturnType.Name == typename)
                        .Select(m => (m.GetDescription(), new Func<T?>(() => (T?)m.Invoke(null, parameters))));
        }
        public static IObservable<Assembly> LoadedAssemblies()
        {
            return Observable
                .FromEventPattern<AssemblyLoadEventHandler, AssemblyLoadEventArgs>(
                a => AppDomain.CurrentDomain.AssemblyLoad += a,
                a => AppDomain.CurrentDomain.AssemblyLoad -= a)
                .Select(a => a.EventArgs.LoadedAssembly);
        }

    }
}
