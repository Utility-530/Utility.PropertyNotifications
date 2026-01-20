using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xaml.Behaviors;

namespace WPF.ComboBoxes.Roslyn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            var grid = new UniformGrid() {  };
            var window = new Window() { Content = grid };
            var compilation = Compiler.Compile(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            grid.Children.Add(makeContent(hcc =>
            {
                hcc.Header = "filters types";
            }, c =>
            {
                var behaviors = Interaction.GetBehaviors(c);
                behaviors.Add(new SymbolBehavior() { Compilation = compilation, Kind = Kind.Type });
            }));
            grid.Children.Add(makeContent(hcc =>
            {
                hcc.Header = "filters methods";
            }, c =>
            {
                var behaviors = Interaction.GetBehaviors(c);
                behaviors.Add(new SymbolBehavior() { Compilation = compilation, Kind = Kind.Method });
            }));
            grid.Children.Add(makeContent(hcc =>
            {
                hcc.Header = "filters types with debugging";
            }, c =>
            {
                c.SetValue(FilterBehavior.IsDebuggingProperty, true);
                var behaviors = Interaction.GetBehaviors(c);
                behaviors.Add(new SymbolBehavior() {  Compilation = compilation, Kind = Kind.Type});
            }));
            grid.Children.Add(makeContent(hcc=>
            {
                hcc.Header = "filters methods then displays their parameters on selection";
            },c =>
            {
                c.SetValue(FilterBehavior.IsSelectionSecondOrderProperty, true);
                var behaviors = Interaction.GetBehaviors(c);
                behaviors.Add(new SymbolBehavior() { Compilation = compilation, Kind = Kind.Method, SecondaryKind = Kind.Parameters });
            }));
            window.Show();
            base.OnStartup(e);
        }

        HeaderedContentControl makeContent(Action<HeaderedContentControl>? actionHCC = null, Action<ComboBox>? action = null)
        {
            var style = this.Resources["HeaderedContentControlStyle"] as Style;
            var headeredContentControl = new HeaderedContentControl();
            headeredContentControl.Style = style;
            actionHCC?.Invoke(headeredContentControl);
            headeredContentControl.Content = makeComboBox(action);
            return headeredContentControl;
        }

        ComboBox makeComboBox(Action<ComboBox>? action = null)
        {
            var style = this.Resources["VisualStudio_Roslyn_Style"] as Style;
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
