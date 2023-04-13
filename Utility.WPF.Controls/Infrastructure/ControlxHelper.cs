using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility.WPF.Controls
{
    public static class ControlxHelper
    {
        public static IObservable<Dictionary<string, object>> Any(this IDictionary<string, ISubject<object>> subjects)
        {
            return Observable.Create<Dictionary<string, object>>(observer =>
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                var sub = new Subject<Dictionary<string, object>>();
                var xx = new List<IDisposable>();
                foreach (var x in subjects)
                {
                    xx.Add(x.Value.Subscribe(_ => { dict[x.Key] = _; sub.OnNext(dict); }));
                }
                return new System.Reactive.Disposables.CompositeDisposable(xx);
            });
        }

        public static ISubject<object> GetSubject(this IDictionary<string, ISubject<object>> subjects, string name)
        {
            subjects[name] = subjects.ContainsKey(name) ? subjects[name] : new Subject<object>(); return subjects[name];
        }

        public static void OnNext(this IDictionary<string, ISubject<object>> subjects, string name, object value)
        {
            subjects[name] = subjects.ContainsKey(name) ? subjects[name] : new Subject<object>();
            subjects[name].OnNext(value);
        }
    }
}