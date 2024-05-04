 using Splat;
using System.Threading;
using Utility.Collections;
using Views.Trees;

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
            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);
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
                StyleSelector = CustomStyleSelector.Instance,
                EventListener = EventListener.Instance
            };
        }
    }
}
