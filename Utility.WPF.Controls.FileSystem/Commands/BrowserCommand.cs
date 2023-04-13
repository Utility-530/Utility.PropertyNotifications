using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.WPF.Command;

namespace Utility.WPF.Controls.FileSystem
{
    public class BrowserCommand : RelayCommand
    {
        public BrowserCommand(Func<IObservable<string>> func) : base(GetAction(func, out var sub))
        {
            sub.Subscribe(a =>
            {
                RaiseTextChangedEvent(a);
            });
        }

        private static Action GetAction(Func<IObservable<string>> func, out ReplaySubject<string> replaySubject)
        {
            replaySubject = new(1);
            ReplaySubject<string> rSubject1 = new(1);
            rSubject1.Subscribe(replaySubject);
            IDisposable? disposable = null;
            return new Action(() =>
            {
                disposable?.Dispose();
                disposable = func().Subscribe(a =>
                rSubject1.OnNext(a));
            });
        }

        protected void RaiseTextChangedEvent(string text)
        {
            this.TextChanged?.Invoke(text);
        }

        public event Action<string> TextChanged;
    }
}