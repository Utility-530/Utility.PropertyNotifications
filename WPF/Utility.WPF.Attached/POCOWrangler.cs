using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Attached
{
    public class POCOWrangler
    {
        #region POCOWrangler.BindPropertyToText Attached Property

        public static string GetBindPropertyToText(TextBox obj)
        {
            return (string)obj.GetValue(BindPropertyToTextProperty);
        }

        public static void SetBindPropertyToText(TextBox obj, PropertyPath value)
        {
            obj.SetValue(BindPropertyToTextProperty, value);
        }

        public static readonly DependencyProperty BindPropertyToTextProperty =
            DependencyProperty.RegisterAttached("BindPropertyToText", typeof(string), typeof(POCOWrangler),
                new PropertyMetadata(null, BindPropertyToText_PropertyChanged));

        private static void BindPropertyToText_PropertyChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string str && dObject is TextBox textBox)
            {
                var binding = new Binding(str);

                //  The POCO object we're editing must be the DataContext of the TextBox,
                //  which is what you've got already -- but don't set Source explicitly
                //  here. Leave it alone and Binding.Source will be updated as
                //  TextBox.DataContext changes. If you set it explicitly here, it's
                //  carved in stone. That's especially a problem if this attached
                //  property gets initialized before DataContext.
                //binding.Source = textBox.DataContext;

                binding.Mode = BindingMode.TwoWay;

                BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            }
        }

        #endregion POCOWrangler.BindPropertyToText Attached Property
    }
}