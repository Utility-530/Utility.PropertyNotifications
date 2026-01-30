using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Utility.Interfaces.NonGeneric;
using Utility.Reactives;

namespace Utility.WPF.Reactives
{
    public static class DependencyObjectHelper
    {
        public static IObservable<EventArgs> Observe<T>(
            this T component,
            DependencyProperty dp)
            where T : DependencyObject
        {
            return Observable.Create<EventArgs>(observer =>
            {
                // Must capture dispatcher now
                var dispatcher = component.Dispatcher;

                EventHandler handler = (s, e) =>
                {
                    if (dispatcher.CheckAccess())
                    {
                        observer.OnNext(e);
                    }
                    else
                    {
                        dispatcher.Invoke(() => observer.OnNext(e));
                    }
                };

                var descriptor = DependencyPropertyDescriptor.FromProperty(dp, typeof(T));

                // Ensure hook-up happens on UI thread
                if (dispatcher.CheckAccess())
                {
                    descriptor.AddValueChanged(component, handler);
                }
                else
                {
                    dispatcher.Invoke(() => descriptor.AddValueChanged(component, handler));
                }

                // Create disposable that removes the handler on UI thread
                return Disposable.Create(() =>
                {
                    if (dispatcher.HasShutdownStarted)
                        return;

                    if (dispatcher.CheckAccess())
                    {
                        descriptor.RemoveValueChanged(component, handler);
                    }
                    else
                    {
                        dispatcher.Invoke(() => descriptor.RemoveValueChanged(component, handler));
                    }
                });
            });
        }

        public static IObservable<TProp> Observe<T, TProp>(
            this T component,
            Expression<Func<T, TProp>> propertyExpression)
            where T : DependencyObject
        {
            var dp = GetDependencyProperty(propertyExpression);
            IObservable<TProp> main() => component.Observe(dp)
                            .Select(_ => GetValueOnUIThread<TProp>(component, dp));
            return dp == null ? main() : main().StartWith((TProp)component.GetValue(dp));
        }

        private static TProp GetValueOnUIThread<TProp>(
            DependencyObject component,
            DependencyProperty dp)
        {
            var dispatcher = component.Dispatcher;

            if (dispatcher.CheckAccess())
                return (TProp)component.GetValue(dp);

            return (TProp)dispatcher.Invoke(() => component.GetValue(dp));
        }

        private static DependencyProperty GetDependencyProperty<T, TProp>(
            Expression<Func<T, TProp>> expression)
        {
            if (expression.Body is not MemberExpression memberExpression)
                throw new ArgumentException("Expression must be a property access.", nameof(expression));

            string propertyName = memberExpression.Member.Name + "Property";

            var field = memberExpression.Member.DeclaringType.GetField(
                propertyName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return field is null
                ? throw new ArgumentException(
                    $"Could not find a DependencyProperty named '{propertyName}'.")
                : (DependencyProperty)field.GetValue(null);
        }
    }
}