using System;
using System.Collections;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using Utility.Collections;
using Utility.Objects;
using Utility.Trees.Abstractions;
using Utility.WPF.Nodes.NewFolder;
using Views.Trees;
using VisualJsonEditor.Test;
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
            SQLitePCL.Batteries.Init();
            Collection.Context = SynchronizationContext.Current;
            TreeViewer treeViewer = TreeViewer();

            var window = new Window { Content = treeViewer };
            window.Show();


            CustomDataTemplateSelector.Instance
                .OfType<string>()
                .Where(a => a.Equals("refresh", StringComparison.InvariantCultureIgnoreCase))
                .Subscribe(a =>
                {
                    TreeViewer treeViewer = TreeViewer();
                    window.Content = treeViewer;
                });
        }

        private static TreeViewer TreeViewer()
        {
            return new TreeViewer
            {
                ViewModel = new RootNode(),
                TreeViewItemFactory = CustomTreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = CustomItemsPanelConverter.Instance,
                DataTemplateSelector = CustomDataTemplateSelector.Instance,
                TreeViewFilter = CustomFilter.Instance,
                StyleSelector = TreeViewItemStyleSelector.Instance
            };
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

        public static CustomFilter Instance { get; } = new();
    }
}
