using System.Windows;
using System.Windows.Controls;
using Utility.Commands;

namespace Utility.WPF.Adorners
{
    public class NullBehavior : MaskBehavior
    {
        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(object), typeof(NullBehavior), new PropertyMetadata());

        public NullBehavior()
        {
            Text = "null";

            MaskCommand = new Command(() =>
            {
                Mask();
            });

            CancelCommand = new Command(() =>
            {
                Cancel();
            });
        }

        public object DefaultValue
        {
            get { return GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        protected virtual void Mask()
        {
            if (AssociatedObject is ContentControl contentControl)
            {
                contentControl.Content = DefaultValue;
            }
            if (AssociatedObject is TextBox textBox)
            {
                textBox.Text = DefaultValue.ToString();
            }
            if (AssociatedObject is TextBlock textBlock)
            {
                textBlock.Text = DefaultValue.ToString();
            }
        }

        protected virtual void Cancel()
        {
            if (AssociatedObject is ContentControl contentControl)
            {
                contentControl.Content = null;
            }
            if (AssociatedObject is TextBox textBox)
            {
                textBox.Text = null;
            }
            if (AssociatedObject is TextBlock textBlock)
            {
                textBlock.Text = null;
            }
        }
    }
}