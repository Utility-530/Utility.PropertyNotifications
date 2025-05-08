﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{
    public record PropertyCall(object Target, object Value, string Name);

    public static partial class PropertyCalledExtensions
    {
        private class PropertyObservable : IObservable<PropertyCall>
        {
            private readonly INotifyPropertyCalled _target;
            private readonly bool includeNonInitial;

            public PropertyObservable(INotifyPropertyCalled target, bool includeNonInitial)
            {
                _target = target;
                this.includeNonInitial = includeNonInitial;
            }

            private class Subscription : IDisposable
            {
                private readonly INotifyPropertyCalled _target;
                private readonly IObserver<PropertyCall> _observer;

                public Subscription(INotifyPropertyCalled target, IObserver<PropertyCall> observer, bool includeNonInitial)
                {
                    _target = target;
                    _observer = observer;

                    if (includeNonInitial)
                        foreach (var item in _target.MissedCalls.ToArray())
                        {
                            onPropertyCalled(target, item);
                        }

                    _target.PropertyCalled += onPropertyCalled;

                    if (_target.MissedCalls is IList<PropertyCall> missedCalls)
                    {
                        missedCalls.Clear();
                    }
                }

                private void onPropertyCalled(object sender, PropertyCalledEventArgs e)
                {
                    _observer.OnNext(new(_target, e.Value, e.PropertyName));
                }

                public void Dispose()
                {
                    _target.PropertyCalled -= onPropertyCalled;
                    _observer.OnCompleted();
                }
            }

            public IDisposable Subscribe(IObserver<PropertyCall> observer)
            {
                return new Subscription(_target, observer, includeNonInitial);
            }
        }

        public static IObservable<PropertyCall> WhenCalled<TModel>(this TModel model,
           bool includeNonInitial = true) where TModel : INotifyPropertyCalled
        {
            return new PropertyObservable(model, includeNonInitial);
        }
    }
}