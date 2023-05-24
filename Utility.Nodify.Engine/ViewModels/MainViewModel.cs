using DryIoc;
using Utility.Nodify.Core;
using Utility.Nodify.Demo.ViewModels;
using System.Linq;
using System.Collections.ObjectModel;
using IContainer = DryIoc.IContainer;

namespace Utility.Nodify.Demo
{
    public class MainViewModel
    {

        private readonly IContainer container;

        public MainViewModel(IContainer container)
        {
            this.container = container;
            TabsViewModel.AddEditorCommand.Execute(container.Resolve<DiagramsViewModel>());

            var selected = container.Resolve<RangeObservableCollection<Diagram>>(serviceKey:Keys.SelectedDiagram);
            selected.CollectionChanged += MainViewModel_CollectionChanged;
            if (selected.FirstOrDefault() is Diagram item)
            {
                TabsViewModel.AddEditorCommand.Execute(container.Resolve<OperationsEditorViewModel>());
            }
        }

        private void MainViewModel_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //foreach (Diagram item in e.NewItems)
            //{
                TabsViewModel.AddEditorCommand.Execute(container.Resolve<OperationsEditorViewModel>());
           // }
        }

        public MessagesViewModel MessagesViewModel => container.Resolve<MessagesViewModel>();
        public TabsViewModel TabsViewModel => container.Resolve<TabsViewModel>();
        //public InterfaceViewModel InterfaceViewModel => container.Resolve<InterfaceViewModel>();


    }
}
