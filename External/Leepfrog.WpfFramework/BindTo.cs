using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Text.RegularExpressions;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Leepfrog.WpfFramework")]

namespace Leepfrog.WpfFramework
{
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class BindToExtension : MarkupExtension
    {
        private Binding _binding;
        private string _path; 
        private IValueConverter _converter;
        private object _param;
        private object _fallback;
        private object _null;
        private string _format;
        private UpdateSourceTrigger _update = UpdateSourceTrigger.Default;
        private BindingMode _mode = BindingMode.Default;

        public IValueConverter Converter { get { return _converter; } set { _converter = value; } }
        public object Param { get { return _param; } set { _param = value; } }
        public object Fallback { get { return _fallback; } set { _fallback = value; } }
        public object Null { get { return _null; } set { _null = value; } }
        public object Nullback { set { _null = value; _fallback = value; } }
        public string Format { get { return _format; } set { _format = value; } }
        public UpdateSourceTrigger Update { get { return _update; } set { _update = value; } }
        public BindingMode Mode { get { return _mode; } set { _mode = value; } }

        public BindToExtension()
        {
        }

        public BindToExtension(string path)
        {
            _path = path;
        }

        public void ProcessPath(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(_path))
            {
                _binding = new Binding();
                return;
            }

            var parts = Regex.Split(_path.Replace("..", ".DataContext.").Replace("..", ".DataContext."), @"\.|(\(.*\..*\))")
                .Where(p => p.Any())
                .Select(p => p.Trim())
                .ToArray();
            if (
                (parts.Any())
             && (String.IsNullOrWhiteSpace(parts[parts.Length - 1]))
                )
            {
                parts[parts.Length - 1] = "DataContext";
            }

            RelativeSource relativeSource = null;
            object source = null;
            string elementName = null;

            var partIndex = 0;

            if (parts[0].StartsWith("-"))
            {
                relativeSource = new RelativeSource(RelativeSourceMode.PreviousData);
                parts[0] = parts[0].Substring(1);
            }
            else if (parts[0].StartsWith("#"))
            {
                elementName = parts[0].Substring(1);
                partIndex++;
            }
            else if (parts[0].ToLower() == "ancestors" || parts[0].ToLower() == "ancestor")
            {
                if (parts.Length < 2) throw new Exception("Invalid path, expected 2 or more identifiers ancestors.#Type#.[Path] (e.g. Ancestors.DataGrid, Ancestors.DataGrid.SelectedItem, Ancestors.DataGrid.SelectedItem.Text)");
                // split by - to get ancestor and level
                var ancestorInfo = parts[1].Split('-');
                var typeString = ancestorInfo[0];
                int typeLevel = 0;

                if (int.TryParse(typeString, out typeLevel))
                {
                    relativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(FrameworkElement), typeLevel);
                }
                else
                {
                    if (!(serviceProvider is IXamlTypeResolver)) // NOTE, this is to prevent the design time editor from showing an error related to the use of TypeExtension
                    {
                        relativeSource = new RelativeSource(RelativeSourceMode.Self);
                    }
                    else
                    {
                        var type = (Type)new System.Windows.Markup.TypeExtension(typeString).ProvideValue(serviceProvider);
                        if (type == null) throw new Exception("Could not find type: " + typeString);
                        if (
                            (ancestorInfo.Length > 1)
                         && (int.TryParse(ancestorInfo[1], out typeLevel))
                           )
                        {
                        }
                        else
                        {
                            typeLevel = 1;
                        }
                        relativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, type, typeLevel);
                    }
                }
                partIndex += 2;
            }
            else if (parts[0].ToLower() == "template" || parts[0].ToLower() == "templateparent" || parts[0].ToLower() == "templatedparent" || parts[0].ToLower() == "templated")
            {
                relativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
                partIndex++;
            }
            else if (parts[0].ToLower() == "thiswindow")
            {
                relativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1);
                partIndex++;
            }
            else if ((parts[0].ToLower() == "this") || (parts[0].ToLower() == "self"))
            {
                relativeSource = new RelativeSource(RelativeSourceMode.Self);
                partIndex++;
            }
            else if (parts[0].ToLower() == "app")
            {
                source = Application.Current;
                partIndex++;
            }

            var partsForPathString = parts.Skip(partIndex);

            var path = string.Join(".", partsForPathString.ToArray());

            if (string.IsNullOrWhiteSpace(path))
            {
                _binding = new Binding();
            }
            else
            {
                _binding = new Binding(path);
            }

            if (source != null)
            {
                _binding.Source = source;
            }

            if (elementName != null)
            {
                _binding.ElementName = elementName;
            }

            if (relativeSource != null)
            {
                _binding.RelativeSource = relativeSource;
            }

            if (_converter != null)
            {
                _binding.Converter = _converter;
            }
            if (_param != null)
            {
                _binding.ConverterParameter = _param;
            }
            if (_fallback != null)
            {
                _binding.FallbackValue = _fallback;
            }
            if (_format != null)
            {
                _binding.StringFormat = _format;
            }
            if (_null != null)
            {
                _binding.TargetNullValue = _null;
            }
            _binding.UpdateSourceTrigger = _update;
            _binding.Mode = _mode;

        }

        public override object ProvideValue(IServiceProvider provider)
        {
            if (_binding == null)
            {
                ProcessPath(provider);
            }
            return _binding.ProvideValue(provider);
        }

    }
}
