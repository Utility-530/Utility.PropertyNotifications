using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;
using Leepfrog.WpfFramework;
using Microsoft.Xaml.Behaviors.Core;

namespace Leepfrog.WpfFramework.Triggers
{
	/// <summary>
	/// Event trigger, with option to mark event as handled
	/// </summary>
	public class EventTrigger : Microsoft.Xaml.Behaviors.EventTrigger
    {

        public string Debug { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public EventTrigger() : base()
        {
        }



        public object OriginalSourceRequired
        {
            get { return (object)GetValue(OriginalSourceRequiredProperty); }
            set { SetValue(OriginalSourceRequiredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OriginalSourceRequired.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalSourceRequiredProperty =
            DependencyProperty.Register("OriginalSourceRequired", typeof(object), typeof(EventTrigger), new PropertyMetadata(null));

        public bool MarkAsHandled { get; set; }

        public EventTrigger(string eventName) : base(eventName)
        {
        }

        protected override void OnEvent(EventArgs eventArgs)
        {
            if (eventArgs is RoutedEventArgs routedEventArgs)
            {
                // added ml 2020-05-03
                // don't process the event if the originalsource is wrong
                if (
                    (OriginalSourceRequired != null)
                 && (routedEventArgs.OriginalSource != OriginalSourceRequired)
                   )
                {
                    return;
                }
                if (MarkAsHandled)
                {
                    var behaviors = Interaction.GetBehaviors(this);
                    var condition = behaviors.FirstOrDefault(b => b is ConditionBehavior) as ConditionBehavior;
                    if (condition?.Condition?.Evaluate() ?? true)
                    {
                        routedEventArgs.Handled = true;
                    }
                }
            }
            base.OnEvent(eventArgs);
        }

    }
}
