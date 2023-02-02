#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace UtilityWpf.Demo.Buttons
{
    /// <summary>
    /// Interaction logic for DualButtonUserControl.xaml
    /// </summary>
    public partial class DualButtonUserControl : UserControl
    {
        public DualButtonUserControl()
        {
            InitializeComponent();

            //this.SwitchControl1
            //    .SelectToggles()
            //    .Select(a => (object?)null)
            //    .InvokeCommand(TransitionViewModel1?.SelectNext);

            //MainViewModelViewHost.ViewModels = Locator.Current.GetService<TransitionViewModel>();
        }
    }

    public class TransitionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ViewModelItem(value?.ToString().Split(".").Last()) { Shape = value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ViewModelItem viewModelItem)
                return viewModelItem.Shape;
            return null;
        }
    }

    public class ViewModelItem : IEquatable<ViewModelItem>
    {
        public ViewModelItem(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public object Shape { get; set; }

        public bool Equals(ViewModelItem? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((ViewModelItem)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }
    }
}