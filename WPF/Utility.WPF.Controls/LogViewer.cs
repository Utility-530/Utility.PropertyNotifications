using System;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Splat;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls
{
    //https://github.com/kentcb/YouIandReactiveUI
    //    [Sample(
    //        "Logging",
    //        @"This sample demonstrates Splat's logging infrastructure.

    //By registering a custom instance of `Splat.ILogger` in the service locator, this sample is able to collect and display all logging calls made by the application.]
    public sealed class LogViewer : Controlx, IEnableLogger
    {
        static LogViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogViewer), new FrameworkPropertyMetadata(typeof(LogViewer)));
            // This is how we register our logger implementation. ReactiveUI and anything else using Splat logging will henceforth call into
            // our logger. You would normally do this registration on application startup, not in a view model like this.
            Locator
                .CurrentMutable
                .RegisterConstant(ObservableLogger.Instance, typeof(ILogger));
        }

        public LogViewer()
        {
            //ControlNames.Add("logOutputTextBox");
            object lck = new object();
            var dis = ObservableLogger

                .Instance
                .Messages

                .Scan(new StringBuilder(), (sb, next) =>
                {
                    return sb.Append("[").Append(next.Item1.ToString()).Append("] ").AppendLine(next.Item2);
                }
                )
                .CombineLatest(this.Control<TextBox>("logOutputTextBox").Select(a =>
                a as TextBox).Where(a => a != null), (a, b) => (a, b))

                .Subscribe(c =>
                {
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            c.b.Text = c.a.ToString();
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                });

            this
                .Log()
                .Info("Initialized.");
        }
    }
}