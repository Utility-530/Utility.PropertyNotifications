using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Windows;
using Utility.Collections;
using Utility.Enums;
using Utility.Nodes.Demo.Infrastructure;
using Utility.Objects;
using Utility.Trees.Abstractions;
using Views.Trees;
using VisualJsonEditor.Test.Infrastructure;

namespace Utility.Nodes.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Collection.Context = SynchronizationContext.Current;
            var treeViewer = new TreeViewer
            {
                ViewModel = new RootPropertyNode(),
                TreeViewItemFactory = CustomTreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = new CustomItemsPanelConverter(),
                DataTemplateSelector = new CustomDataTemplateSelector(),
                TreeViewFilter = new CustomFilter()
            };

            var window = new Window { Content = treeViewer };
            window.Show();
        }
    }

    internal class CustomFilter : ITreeViewFilter
    {
        public bool Filter(object item)
        {
            if (item is MethodNode { Data: CommandValue { Instance: { } instance, MethodInfo.Name: { } name } })
            {
                if (instance.GetType().IsArray)
                {
                    return false;
                }
            }


            if (item is IReadOnlyTree { Data: PropertyData { Descriptor: { ComponentType: { } componentType, DisplayName: { } displayName } descriptor } propertyNode })
            {
                if (componentType.Name == "Array")
                {
                    if (displayName == "IsFixedSize")
                        return false;
                    if (displayName == "IsReadOnly")
                        return false;
                    if (displayName == "IsSynchronized")
                        return false;
                    if (displayName == "LongLength")
                        return false;
                    if (displayName == "Length")
                        return false;
                    if (displayName == "Rank")
                        return false;
                    if (displayName == "SyncRoot")
                        return false;
                }
                return true;
            }

            return true;
        }
    }

    public class CustomItemsPanelConverter : ItemsPanelConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IReadOnlyTree { Data: PropertyData { Descriptor: { PropertyType:{ }propertyType, ComponentType: { } componentType, DisplayName: { } displayName } descriptor } baseObject })
            {
                if(propertyType == typeof(LEDMessage))
                {
                    return convert(new ItemsPanel
                    {
                        Type = Arrangement.Stacked,
                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                    });
                }
            }

            return base.Convert(value, targetType, parameter, culture);
        }
    }
}
