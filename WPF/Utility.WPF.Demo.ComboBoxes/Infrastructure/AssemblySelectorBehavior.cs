using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Enums;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes;
using Utility.Trees.Abstractions;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Converters;
using Utility.WPF.Factorys;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.ComboBoxes
{
    public class AssemblySelectorBehavior : TreeSelectorBehavior
    {
        class Converter :IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is Assembly assembly)
                {
                    return assembly.GetName().Name;
                }
                else if (value is string s)
                {
                    return s;
                }
                return DependencyProperty.UnsetValue;
            }
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            public static Converter Instance { get; } = new Converter();
        }

        private static Assembly? entryAssembly;
        private static string entryLocation;
        private static string? appDirectory;

        protected override void OnAttached()
        {
            AssociatedObject.DropDownOpened += (s, e) =>
            {
                if (AssociatedObject.ItemsSource is null)
                {
                    var x = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .GroupBy(a => group(a))
                    .Select(data => new Model(() => data.Select(a => new Model() { Data = a}).ToArray()) { Data = data.Key, IsExpanded = false }).ToArray();
                    NodeRoot.Create(x).Subscribe();
                    AssociatedObject.ItemsSource = x;
                }
            };
            base.OnAttached();
        }

        public INodeRoot NodeRoot { get; set; }


        public override DataTemplateSelector DataTemplateSelector()
        {
            DataTemplate hierarchicalDataTemplate = null;
            return DataTemplateHelper.CreateSelector((a, b) =>
            {
                return hierarchicalDataTemplate ??= TemplateGenerator.CreateHierarcialDataTemplate(() =>
                {
                    var contentControl = new ContentControl();
                    contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath(nameof(IGetData.Data)),
                        Converter = Converter.Instance });
                    return contentControl;
                }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Children)) });
            });
        }

        public override DataTemplateSelector SelectedTemplateSelector() => DataTemplateSelector();

        static string group(Assembly assembly)
        {
            entryAssembly ??= Assembly.GetEntryAssembly();
            entryLocation ??= entryAssembly?.Location ?? "";
            appDirectory ??= string.IsNullOrEmpty(entryLocation) ? AppDomain.CurrentDomain.BaseDirectory : Path.GetDirectoryName(entryLocation);

            if (IsSystemAssembly(assembly))
                return "System";
            if (IsUserAssembly(assembly, appDirectory, entryAssembly))
                return "User";
            return "Other";
        }

        static bool IsSystemAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            var publicKeyToken = assembly.GetName().GetPublicKeyToken();

            // Check if it's a Microsoft assembly by public key token
            var microsoftTokens = new[]
            {
                "b77a5c561934e089", // Microsoft
                "b03f5f7f11d50a3a", // Microsoft
                "31bf3856ad364e35", // Microsoft
                "cc7b13ffcd2ddd51"  // Microsoft
            };

            var tokenString = publicKeyToken != null && publicKeyToken.Length > 0
                ? BitConverter.ToString(publicKeyToken).Replace("-", "").ToLower()
                : "";

            if (microsoftTokens.Contains(tokenString))
                return true;

            // Check common system assembly prefixes
            var systemPrefixes = new[]
            {
                "System",
                "Microsoft",
                "mscorlib",
                "netstandard",
                "WindowsBase",
                "PresentationCore",
                "PresentationFramework",
                "UIAutomationTypes"
            };

            return systemPrefixes.Any(prefix => name.StartsWith(prefix));
        }

        static bool IsUserAssembly(Assembly assembly, string appDirectory, Assembly entryAssembly)
        {
            if (assembly == entryAssembly)
                return true;

            var location = assembly.Location;

            return assembly.GetName().Name.Contains("Utility");
                
            //// If location is empty or dynamic assembly
            //if (string.IsNullOrEmpty(location))
            //    return false;

            //// Check if assembly is in the application directory
            //if (!string.IsNullOrEmpty(appDirectory) && location.StartsWith(appDirectory, StringComparison.OrdinalIgnoreCase))
            //{
            //    // Exclude if it's in a packages or external directory
            //    var relativePath = location.Substring(appDirectory.Length).ToLower();
            //    if (relativePath.Contains("packages") || relativePath.Contains("nuget"))
            //        return false;

            //    return true;
            //}

            return false;
        }
    }
}
