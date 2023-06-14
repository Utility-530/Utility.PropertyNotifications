using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Dragablz
{
    public class ButtonsControl : DragablzVerticalItemsControl
    {
        public static readonly DependencyProperty CommandPathProperty =
            DependencyProperty.Register("CommandPath", typeof(string), typeof(ButtonsControl), new PropertyMetadata(null));

        static ButtonsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonsControl), new FrameworkPropertyMetadata(typeof(ButtonsControl)));
        }

        public string CommandPath
        {
            get { return (string)GetValue(CommandPathProperty); }
            set { SetValue(CommandPathProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not Control control)
                return;
            _ = control.ApplyTemplate();
            if (element.ChildOfType<Button>() is not Button button)
                return;
            if (string.IsNullOrEmpty(CommandPath) == false)
                BindingOperations.SetBinding(button, Button.CommandProperty, CreateCommandBinding());

            if (button.Content is not TextBlock textBlock)
                return;
            if (string.IsNullOrEmpty(DisplayMemberPath) == false)
                BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, CreateBinding());

            base.PrepareContainerForItemOverride(element, item);

            Binding CreateBinding()
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = BindingMode.OneWay,
                };
            }

            Binding CreateCommandBinding()
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(CommandPath),
                    Mode = BindingMode.OneWay,
                };
            }
        }
    }
}