using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Controls
{
    public class GuidBox : Control
    {
        private TextBox EditBox;
        private Control SpacedBox;

        public static readonly DependencyProperty GuidProperty =
            DependencyProperty.Register("Guid", typeof(Guid), typeof(GuidBox), new PropertyMetadata(Changed)),
            GuidAsStringProperty = DependencyProperty.Register("GuidAsString", typeof(string), typeof(GuidBox), new PropertyMetadata(Changed)),
            IsLowerCaseProperty = DependencyProperty.Register("IsLowerCase", typeof(bool), typeof(GuidBox), new PropertyMetadata(true, Changed)),
            IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(GuidBox), new PropertyMetadata(IsReadOnlyChanged));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GuidBox guidBox))
                return;
            if (e.NewValue is bool b)
            {
                if (guidBox.SpacedBox != null)
                    guidBox.SpacedBox.IsEnabled = !b;
            }
        }

        static GuidBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GuidBox), new FrameworkPropertyMetadata(typeof(GuidBox)));
        }

        public GuidBox()
        {

        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GuidBox guidBox))
                return;

            if (e.NewValue is Guid guid)
            {
                guidBox.GuidAsString = guidBox.IsLowerCase ? guid.ToString() : guid.ToString().ToUpper();
                guidBox.SpacedBox?.InvalidateVisual();
            }
            if (e.NewValue is string guidAsString)
            {
                if (Guid.TryParse(guidAsString, out Guid guid_))
                {
                    guidBox.Guid = guid_;
                }
            }
            if (e.NewValue is bool b)
            {
                guidBox.GuidAsString = b ? guidBox.Guid.ToString() : guidBox.Guid.ToString().ToUpper();
                guidBox.SpacedBox?.InvalidateVisual();
            }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }


        public Guid Guid
        {
            get { return (Guid)GetValue(GuidProperty); }
            set { SetValue(GuidProperty, value); }
        }



        public bool IsLowerCase
        {
            get { return (bool)GetValue(IsLowerCaseProperty); }
            set { SetValue(IsLowerCaseProperty, value); }
        }


        public string GuidAsString
        {
            get { return (string)GetValue(GuidAsStringProperty); }
            set { SetValue(GuidAsStringProperty, value); }
        }




        public override void OnApplyTemplate()
        {
            (GetTemplateChild("Copy") as Button).Click += GuidBox_Click;
            (GetTemplateChild("Generate") as Button).Click += (s, e) => GenerateGuid();
            EditBox = GetTemplateChild("EditBox") as TextBox;
            SpacedBox = GetTemplateChild("SpacedBox") as Control;
            EditBox.LostFocus += GuidBox_LostFocus;
            EditBox.GotFocus += GuidBox_GotFocus;
            SpacedBox.IsEnabled = !IsReadOnly;
            GuidAsString = IsLowerCase ? Guid.ToString() : Guid.ToString().ToUpper();
            base.OnApplyTemplate();

            void GuidBox_Click(object sender, RoutedEventArgs e)
            {
                Clipboard.SetText(GuidAsString);
            }

            void GenerateGuid()
            {
                Guid = Guid.NewGuid();
            }

            void GuidBox_GotFocus(object sender, RoutedEventArgs e)
            {
                EditBox.IsReadOnly = IsReadOnly;
            }

            void GuidBox_LostFocus(object sender, RoutedEventArgs e)
            {
                EditBox.IsReadOnly = true;
            }

        }
    }


    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                return !(bool)value;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
