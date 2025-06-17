using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Behaviors
{

    public class TaskCompletionBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            if (Result is not null)
                AssociatedObject.SetValue(Property, Result);
            base.OnAttached();
        }

        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register("Property", typeof(DependencyProperty), typeof(TaskCompletionBehavior), new PropertyMetadata());

        public static readonly DependencyProperty ResultProperty =
    DependencyProperty.Register("Result", typeof(object), typeof(TaskCompletionBehavior), new PropertyMetadata());


        public static readonly DependencyProperty IsCompletedProperty =
            DependencyProperty.Register(
                nameof(IsCompleted),
                typeof(bool),
                typeof(TaskCompletionBehavior),
                new PropertyMetadata(false));


        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register(
                nameof(Task),
                typeof(Task),
                typeof(TaskCompletionBehavior),
                new PropertyMetadata(null, OnTaskChanged));

        public Task Task
        {
            get => (Task)GetValue(TaskProperty);
            set => SetValue(TaskProperty, value);
        }

        public bool IsCompleted
        {
            get => (bool)GetValue(IsCompletedProperty);
            set => SetValue(IsCompletedProperty, value);
        }

        public DependencyProperty Property
        {
            get { return (DependencyProperty)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }

        public object Result
        {
            get { return (object)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        private static async void OnTaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (TaskCompletionBehavior)d;
            var task = e.NewValue as Task;

            if (task == null)
            {
                behavior.IsCompleted = false;

                return;
            }

            try
            {
                await task.ConfigureAwait(true);
                var type = task.GetType();
                if (type.GenericTypeArguments().Length != 0)
                {
                    behavior.Result = type.GetProperty("Result").GetValue(task);
                    behavior.AssociatedObject?.SetValue(behavior.Property, behavior.Result);
                }
            }
            catch (Exception ex)
            {
                behavior.IsCompleted = false;
                MessageBox.Show(ex.Message);
                // Optionally expose error state in another DP
            }
        }
    }
}
