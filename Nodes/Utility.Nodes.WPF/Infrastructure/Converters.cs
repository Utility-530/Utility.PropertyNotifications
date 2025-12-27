using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Utility.PropertyDescriptors;
using Utility.Interfaces.NonGeneric;
using Utility.Meta;
using Utility.Models;
using Utility.Models.Trees;
using Utility.WPF;
using Utility.WPF.Controls.ComboBoxes;
using System.Text.RegularExpressions;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodes.WPF
{
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
            if (value is ChangeRoutedEventArgs { Type: Changes.Type.Add, Instance: INodeViewModel nObject } args)
            {
                args.Handled = true;
                if (nObject is Model<string> model && nObject is IGetParent<INodeViewModel> { Parent: ListModel { Children: { } collection } dModels })
                {
                    //var alias = toFileName(dModels, model);
                    var newName = StringExtensions.IncrementSuffixNumber(model.Get());
                    //var filePath = Utility.Helpers.PathHelper.ChangeFilename(model.FilePath, alias);
                    return new Model<string> { Name = newName, Value = model.Value };
                }
                throw new NotImplementedException("V FDF$3");
            }
            return DependencyProperty.UnsetValue;

            //static string toFileName(DataFilesModel dataFilesModel, DataFileModel dataFileModel)
            //{
            //    var match = Regex.Match(dataFileModel.TableName, "(.*)_(\\d)$");
            //    return match.Success ?
            //        name(match.Groups[1].Value, dataFilesModel.Children) :
            //        name(dataFileModel.Alias, dataFilesModel.Children);

            //    static string name(string name, IEnumerable dataFilesModel)
            //    {
            //        if (_match(name, dataFilesModel) is { } match)
            //        {
            //            return name + "_" + (int.Parse(match.Groups[1].Value) + 1);
            //        }
            //        return name + "_1";
            //        static Match? _match(string x, IEnumerable dataFilesModel)
            //        {
            //            return dataFilesModel.Cast<DataFileModel>().Select(a => Regex.Match(a.Name, $"{x}_(\\d)$")).Where(a => a.Success).OrderBy(a => a.Groups[2]).LastOrDefault();
            //        }
            //    }


            //}
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
            if (value is ComboBoxTreeView.SelectedNodeEventArgs { Value: Model { Data: Type type } })
            {
                return DescriptorFactory.CreateRoot(type);
            }
            else if (value is ComboBoxTreeView.SelectedNodeEventArgs { Value: AssemblyModel { Assembly: Assembly ass } })
            {
                return DependencyProperty.UnsetValue;
            }
            throw new Exception("V dfr44");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class StringExtensions
    {
        private static readonly Regex NumberAtEnd = new Regex(@"(\d+)$", RegexOptions.Compiled);

        /// <summary>
        /// Adds a number to a string or increments the trailing number if present.
        /// </summary>
        public static string IncrementSuffixNumber(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var match = NumberAtEnd.Match(input);
            if (!match.Success)
            {
                // No number at the end → append "1"
                return input + "1";
            }

            // Extract the numeric part
            var numberPart = match.Groups[1].Value;
            var prefix = input.Substring(0, match.Index);

            // Parse and increment
            if (int.TryParse(numberPart, out int number))
            {
                var incremented = (number + 1).ToString($"D{numberPart.Length}");
                // "D{length}" keeps leading zeros
                return prefix + incremented;
            }

            // Fallback — if number parsing failed, just append "1"
            return input + "1";
        }
    }
}
