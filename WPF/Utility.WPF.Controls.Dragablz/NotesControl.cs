using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Dragablz;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Dragablz
{
    public class NotesControl : DragablzVerticalItemsControl
    {
        static NotesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotesControl), new FrameworkPropertyMetadata(typeof(NotesControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;
            if (element is not Control control)
                return;
            _ = control.ApplyTemplate();
            if (element.ChildOfType<TextBox>() is not TextBox textBox)
                return;

            BindingOperations.SetBinding(textBox, TextBox.TextProperty, CreateBinding(item));

            textBox.MouseLeftButtonDown += TextBox_MouseLeftButtonDown;
            textBox.GotFocus += TextBox_GotFocus;
            base.PrepareContainerForItemOverride(element, item);

            Binding CreateBinding(object item)
            {
                return new Binding
                {
                    Source = item,
                    Path = new PropertyPath(DisplayMemberPath),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }

        private void TextBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as TextBox)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }
    }
}