using Splat;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Utility.Descriptors;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Meta;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Repos;
using Utility.Trees.Abstractions;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Controls.Trees;

namespace Utility.Nodes.Demo.Styles
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
            if (value is ComboBoxTreeView.SelectedNodeEventArgs { Value: IReadOnlyTree { Data: TypeModel { Type: Type type } } })
            {
                if (parameter is IKey { Key: string key } && Guid.TryParse(key, out var guid))
                {
                    var xx = CreateRoot(type, guid);       
                    return xx;
                }
            }
            else if (value is ComboBoxTreeView.SelectedNodeEventArgs { Value: IReadOnlyTree { Data: AssemblyModel { Assembly: Assembly ass } } })
            {
                return DependencyProperty.UnsetValue;
            }
            throw new Exception("V dfr44");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ValuePropertyDescriptor CreateRoot(Type type, Guid guid)
        {
            //var instance = Activator.CreateInstance(type);
            var instance = ActivateAnything.Activate.New(type);
            var rootDescriptor = new RootDescriptor(type) { };
            rootDescriptor.SetValue(null, instance);
            var max = Locator.Current.GetService<ITreeRepository>().MaxIndex(guid, RootDescriptor.DefaultName);
            var x = Locator.Current.GetService<ITreeRepository>().InsertByParent(guid, RootDescriptor.DefaultName, index: max).Take(1).ToTask().Result;
            var root = CreateRoot(rootDescriptor);
            return root;

            ValuePropertyDescriptor CreateRoot(System.ComponentModel.PropertyDescriptor descriptor)
            {
                var _descriptor = DescriptorConverter.ToDescriptor(instance, descriptor);
                _descriptor.Guid = x;
                return _descriptor;
            }
        }
    }

}
