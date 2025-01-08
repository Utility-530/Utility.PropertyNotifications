using System.Globalization;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.Descriptors;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Filters;
using Utility.Trees.Abstractions;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Controls.Trees;

namespace Utility.Nodes.Demo.Filters
{
    public class NewObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = (value as Model).Proliferation().FirstOrDefault();
            return x;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class FinishEditConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NewObjectRoutedEventArgs { IsAccepted: true, NewObject: { } nObject } args)
            {
                return nObject;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedNodeChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ComboBoxTreeView.SelectedNodeEventArgs { Value: IReadOnlyTree { Data: Type type } obj } args)
            {
                if (parameter is IGuid { Guid: { } guid })
                {
                    var xx = CreateRoot(type, guid);
                    return xx;
                }
            }
            throw new Exception("V dfr44");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static IValueDescriptor CreateRoot(Type type, Guid guid)
        {
            var instance = Activator.CreateInstance(type);
            var rootDescriptor = new RootDescriptor(type) { };
            rootDescriptor.SetValue(null, instance);
            var root = CreateRoot(rootDescriptor);
            return root;

            IValueDescriptor CreateRoot(System.ComponentModel.PropertyDescriptor descriptor)
            {
                var _descriptor = DescriptorConverter.ToDescriptor(instance, descriptor);
                _descriptor.Guid = guid;
                return _descriptor;
            }
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IReadOnlyTree { Data: IDescriptor descriptor })
            {
                return Utility.Trees.Demo.Filters.TreeViewFilter.Instance.Convert(value) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
