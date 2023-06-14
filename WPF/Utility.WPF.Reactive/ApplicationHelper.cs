using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace Utility.WPF.Reactive
{
    public static class ApplicationHelper
    {
        public static IObservable<ExitEventArgs> GetExitEvents(this Application app) =>
            Observable
            .FromEventPattern<ExitEventHandler, ExitEventArgs>(h => app.Exit += h, h => app.Exit -= h)
           .Select(a => a.EventArgs);
    }
}