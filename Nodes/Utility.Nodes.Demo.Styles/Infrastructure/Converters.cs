using System.Globalization;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Utility.PropertyDescriptors;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Meta;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Trees.Abstractions;
using Utility.WPF;
using Utility.WPF.Controls.ComboBoxes;
using Utility.Interfaces.Exs;
using System.Text.RegularExpressions;
using System.IO;

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
            if (value is EditRoutedEventArgs { IsAccepted: true, Edit: { } nObject } args)
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

    public class AddDataFileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChangeRoutedEventArgs { Type: Changes.Type.Add, Instance: INode nObject } args)
            {
                args.Handled = true;
                if (nObject is { Data: DataFileModel model, Parent.Data: DataFilesModel { Collection: { } collection } dModels })
                {
                    var alias = toFileName(dModels, model);

                    var filePath = Utility.Helpers.PathHelper.ChangeFilename(model.FilePath, alias);
                    return new DataFileModel { Name = alias, Alias = alias, FilePath = filePath };
                }
                throw new NotImplementedException("V FDF$3");
            }
            return DependencyProperty.UnsetValue;

            static string toFileName(DataFilesModel dataFilesModel, DataFileModel dataFileModel)
            {
                var match = Regex.Match(dataFileModel.Alias, "(.*)_(\\d)$");
                return match.Success ?
                    name(match.Groups[1].Value, dataFilesModel.Collection) :
                    name(dataFileModel.Alias, dataFilesModel.Collection);

                static string name(string name, IEnumerable<DataFileModel> dataFilesModel)
                {
                    if (_match(name, dataFilesModel) is { } match)
                    {
                        return name + "_" + (int.Parse(match.Groups[1].Value) + 1);
                    }
                    return name + "_1";
                    static Match? _match(string x, IEnumerable<DataFileModel> dataFilesModel)
                    {
                        return dataFilesModel.Select(a => Regex.Match(a.Name, $"{x}_(\\d)$")).Where(a => a.Success).OrderBy(a => a.Groups[2]).LastOrDefault();
                    }
                }


            }
        }




        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RemoveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChangeRoutedEventArgs { Type: Changes.Type.Remove, Instance: { } nObject } args)
            {
                args.Handled = true;
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
                    var root = CreateRoot(type, guid);
                    return root;
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

        public static IDescriptor CreateRoot(Type type, Guid guid)
        {
            //var instance = Activator.CreateInstance(type);
            var instance = ActivateAnything.Activate.New(type);
            var rootDescriptor = new RootDescriptor(type) { };
            rootDescriptor.SetValue(null, instance);
            var root = CreateRoot(rootDescriptor);
            return root;

            IDescriptor CreateRoot(System.ComponentModel.PropertyDescriptor descriptor)
            {
                var _descriptor = DescriptorConverter.ToDescriptor(instance, descriptor);
                return _descriptor;
            }
        }
    }

}
