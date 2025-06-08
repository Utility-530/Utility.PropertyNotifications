using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;

namespace Utility.Helpers.Ex
{
    public static class ReflectionHelper
    {
        public static IObservable<Assembly> SelectAssemblies()
        {
            return Observable.Create<Assembly>(obs =>
            {
                var dis1 = ReflectionHelper.LoadedAssemblies().Subscribe(obs);
                var dis2 = Task.Run(() => Reflection.ReflectionHelper.GetAssemblies())
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
