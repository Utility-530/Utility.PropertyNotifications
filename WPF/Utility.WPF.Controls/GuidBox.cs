using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static System.Windows.DependencyProperty;

namespace Utility.WPF.Controls
{
    public class GuidBox : Control
    {
        private TextBox EditBox;
        private Control SpacedBox;
        private Grid? Grid;

        public static readonly DependencyProperty GuidProperty = Register("Guid", typeof(Guid), typeof(GuidBox), new PropertyMetadata(Changed)),
            GuidAsStringProperty = Register("GuidAsString", typeof(string), typeof(GuidBox), new PropertyMetadata("00000000-0000-0000-0000-000000000000", Changed)),
            IsLowerCaseProperty = Register("IsLowerCase", typeof(bool), typeof(GuidBox), new PropertyMetadata(true, Changed)),
            IsReadOnlyProperty = Register("IsReadOnly", typeof(bool), typeof(GuidBox), new PropertyMetadata(true, IsReadOnlyChanged));

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GuidBox guidBox))
                return;
        }

        static GuidBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GuidBox), new FrameworkPropertyMetadata(typeof(GuidBox)));
        }

        public GuidBox()
        {
            FontSize = 8;
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

        private MenuItem lower, upper;
        private Grid? guidBox;

        public override void OnApplyTemplate()
        {
            guidBox = GetTemplateChild("GuidGrid") as Grid;
            var items = guidBox.ContextMenu.Items;
            MenuItems(items);

            EditBox = GetTemplateChild("EditBox") as TextBox;
            SpacedBox = GetTemplateChild("SpacedBox") as Control;
            Grid = GetTemplateChild("Grid") as Grid;
            this.Grid.MouseDown += Grid_MouseDown;
            EditBox.LostFocus += GuidBox_LostFocus;
            EditBox.GotFocus += GuidBox_GotFocus;
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
                IsReadOnly = false;
            }

            void GuidBox_LostFocus(object sender, RoutedEventArgs e)
            {
                IsReadOnly = true;
            }

            void MenuItems(ItemCollection items)
            {
                foreach (var item in items)
                {
                    if (item is MenuItem menuItem)
                    {
                        if (menuItem.Header.Equals("_Copy"))
                            menuItem.Click += GuidBox_Click;
                        else if (menuItem.Header.Equals("_Generate"))
                            menuItem.Click += (s, e) => GenerateGuid();
                        else if (menuItem.Header.Equals("_Lower"))
                        {
                            lower = menuItem;
                            if (IsLowerCase)
                            {
                                lower.IsChecked = true;
                            }
                            lower.Click += (s, e) => { upper.IsChecked = false; this.IsLowerCase = true; };
                        }
                        else if (menuItem.Header.Equals("_Upper"))
                        {
                            upper = menuItem;
                            if (IsLowerCase == false)
                            {
                                upper.IsChecked = true;
                            }
                            upper.Click += (s, e) => { lower.IsChecked = false; this.IsLowerCase = false; };
                        }
                        else if (menuItem.Header == null)
                        {
                            MenuItems(menuItem.Items);
                        }
                    }
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            guidBox.ContextMenu.PlacementTarget = Grid;
            guidBox.ContextMenu.IsOpen = true;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Grid.Visibility = Visibility.Visible;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Grid.Visibility = Visibility.Collapsed;
            base.OnMouseLeave(e);
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