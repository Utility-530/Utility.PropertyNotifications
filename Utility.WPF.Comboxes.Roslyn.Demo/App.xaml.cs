using System;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xaml.Behaviors;
using Utility.PropertyNotifications;
using Utility.Roslyn;
using Utility.WPF.ComboBoxes.Roslyn.Demo.Infrastructure;
using Utility.WPF.ComboBoxes.Roslyn.Infrastructure;

namespace Utility.WPF.ComboBoxes.Roslyn.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            var grid = new UniformGrid() { Rows = 2, Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")) };
            var window = new Window() { Content = grid };
            var entryAssembly = Assembly.GetEntryAssembly();
            string exePath = entryAssembly.Location;

            var basic = new Lazy<CSharpCompilation>(() => Compiler.Compile([exePath]));
            var compilation = new Lazy<CSharpCompilation>(() => Compiler.Compile(kind: Basic.Reference.Assemblies.ReferenceAssemblyKind.Net80));
            //   var compilation = new Lazy<CSharpCompilation>(() => Compiler.Compile(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>()));

            grid.Children.Add(makeContent(hcc =>
            {
                hcc.Header = "filters types";
            }, c =>
            {
                var behaviors = Interaction.GetBehaviors(c);
                behaviors.Add(new SymbolBehavior() { Compilation = compilation.Value, Kind = Kind.Type });
            }));
            grid.Children.Add(makeContent(hcc =>
            {
                hcc.Header = "filters methods";
            }, c =>
            {
                var behaviors = Interaction.GetBehaviors(c);
                behaviors.Add(new SymbolBehavior() { Compilation = compilation.Value, Kind = Kind.Method });
            }));
            grid.Children.Add(makeContent(hcc =>
            {
                hcc.Header = "filters types with debugging";
            }, c =>
            {
                c.SetValue(FilterBehavior.IsDebuggingProperty, true);
                var behaviors = Interaction.GetBehaviors(c);
                behaviors.Add(new SymbolBehavior() { Compilation = compilation.Value, Kind = Kind.Type });
            }));

            grid.Children.Add(makeContent(hcc =>
            {
                hcc.Header = "filter properties";
            }, c =>
            {
                if (this.Resources["CustomToggleButtonStyle"] is not Style style)
                    throw new Exception("!E£F");
                c.Style = style;
                var reflectionBehavior = new ReflectionBehavior() { Type = typeof(Test), FilterType = typeof(bool) };
                var isbooleanFilterTypeViewModel = new BooleanViewModel() { Name = "Filter By Boolean" };                
                isbooleanFilterTypeViewModel
                .WithChangesTo(a => a.Value)
                .Subscribe(isComplex =>
                {
                    if (isComplex)
                    {
                        reflectionBehavior.FilterType = typeof(bool);
                    }
                    else
                    {
                        reflectionBehavior.FilterType = null;
                    }
                });
                var behaviors = Interaction.GetBehaviors(c);
                c.SetValue(FilterBehavior.FiltersProperty, new NotifyPropertyClass[] { isbooleanFilterTypeViewModel });
                c.SetValue(FilterBehavior.FiltersTemplateSelectorProperty, makeCustomDataTemplateSelector());
                behaviors.Add(reflectionBehavior);
            }));

            grid.Children.Add(makeContent(hcc =>
            {
                hcc.Header = "filters methods then displays their parameters on selection";
            }, c =>
            {
                if (this.Resources["CustomWhiteBackgroundStyle"] is not Style style)
                    throw new Exception("!E£F");
                c.Style = style;

                var complexViewModel = new BooleanViewModel() { Name = "Include External References" };
                var symbolBehavior = new SymbolBehavior() { Kind = Kind.Method, SecondaryKind = Kind.Parameters };
                complexViewModel.WithChangesTo(a => a.Value).Subscribe(isComplex =>
                {
                    if (isComplex)
                    {
                        symbolBehavior.Compilation = compilation.Value;
                    }
                    else
                    {
                        symbolBehavior.Compilation = basic.Value;
                    }
                });
                var typeViewModel = new EnumViewModel();
                typeViewModel.WithChangesTo(a => a.Value).Subscribe(a =>
                {
                    symbolBehavior.Type = a != null ? CommonTypesHelper.ToType(a.Value) : null;
                });
                c.SetValue(FilterBehavior.FiltersProperty, new NotifyPropertyClass[] { complexViewModel, typeViewModel });
                c.SetValue(FilterBehavior.FiltersTemplateSelectorProperty, makeCustomDataTemplateSelector());
                var behaviors = Interaction.GetBehaviors(c);
                behaviors.Add(symbolBehavior);
            }));
            window.Show();
            base.OnStartup(e);
        }

        CustomDataTemplateSelector makeCustomDataTemplateSelector() => new CustomDataTemplateSelector()
        {
            EnumTemplate = this.Resources["EnumTemplate"] as DataTemplate,
            BooleanTemplate = this.Resources["BooleanTemplate"] as DataTemplate
        };

        HeaderedContentControl makeContent(Action<HeaderedContentControl>? actionHCC = null, Action<ComboBox>? action = null)
        {
            if (this.Resources["HeaderedContentControlStyle"] is not Style style)
                throw new Exception("!E£F");
            var headeredContentControl = new HeaderedContentControl() { Style = style };
            actionHCC?.Invoke(headeredContentControl);
            headeredContentControl.Content = makeComboBox(action);
            return headeredContentControl;
        }

        ComboBox makeComboBox(Action<ComboBox>? action = null)
        {
            if (this.Resources["VisualStudio_Roslyn_Style"] is not Style style)
                throw new Exception("!E£F");
            var comboBox = new ComboBox() { Style = style };
            comboBox.SetValue(VirtualizingPanel.IsVirtualizingProperty, true);
            comboBox.SetValue(VirtualizingPanel.ScrollUnitProperty, ScrollUnit.Pixel);
            comboBox.SetValue(VirtualizingPanel.VirtualizationModeProperty, VirtualizationMode.Recycling);
            var behaviors = Interaction.GetBehaviors(comboBox);
            behaviors.Add(new FilterBehavior());
            action?.Invoke(comboBox);
            return comboBox;
        }
    }
}
