using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Attached
{
    public class MouseDoubleClick : DependencyObject
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseDoubleClick), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(MouseDoubleClick), new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static object GetCommand(DependencyObject target)
        {
            return target.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is Control control)
            {
                if (e.NewValue != null && e.OldValue == null)
                {
                    control.MouseDoubleClick += OnMouseDoubleClick;
                }
                else if (e.NewValue == null && e.OldValue != null)
                {
                    control.MouseDoubleClick -= OnMouseDoubleClick;
                }
            }
        }

        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Control control)
                return;
            ICommand command = (ICommand)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            if (command.CanExecute(null))
            {
                command.Execute(commandParameter);
                e.Handled = true;
            }
        }
    }

    public class MouseSingleClick : DependencyObject
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseSingleClick), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(MouseSingleClick), new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static object GetCommand(DependencyObject target)
        {
            return target.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is Control control)
            {
                if (e.NewValue != null && e.OldValue == null)
                {
                    control.MouseDown += OnMouseClick;
                }
                else if (e.NewValue == null && e.OldValue != null)
                {
                    control.MouseDown -= OnMouseClick;
                }
            }
        }

        private static void OnMouseClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Control control)
                return;

            ICommand command = (ICommand)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            if (command.CanExecute(null))
            {
                command.Execute(commandParameter);
                e.Handled = true;
            }
        }
    }
}