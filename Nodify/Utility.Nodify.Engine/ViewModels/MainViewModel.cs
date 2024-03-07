using DryIoc;
using Utility.Nodify.Core;
using System.Linq;
using System.Collections.ObjectModel;
using IContainer = DryIoc.IContainer;
using Utility.Nodify.Demo;
using Utility.Nodify.Engine.Infrastructure;
using System;

namespace Utility.Nodify.Engine.ViewModels;

public class MainViewModel
{

    private readonly IContainer container;

    public MainViewModel(IContainer container)
    {
        this.container = container;
        TabsViewModel.AddEditorCommand.Execute(container.Resolve<DiagramsViewModel>());

        var selected = container.Resolve<RangeObservableCollection<Diagram>>(serviceKey: Keys.SelectedDiagram);
        selected.CollectionChanged += MainViewModel_CollectionChanged;
        if (selected.FirstOrDefault() is Diagram item)
        {
            //var diagramViewModel = container.Resolve<DiagramViewModel>();
            try
            {
                var diagramViewModel = container.Resolve<IConverter>().Convert(item);
                container.RegisterInstance(diagramViewModel);
                TabsViewModel.AddEditorCommand.Execute(diagramViewModel);
            }
            catch(Exception ex)
            {

            }
        }
    }

    private void MainViewModel_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        //foreach (Diagram item in e.NewItems)
        //{
        TabsViewModel.AddEditorCommand.Execute(container.Resolve<DiagramViewModel>());
        // }
    }

    public MessagesViewModel MessagesViewModel => container.Resolve<MessagesViewModel>();
    public TabsViewModel TabsViewModel => container.Resolve<TabsViewModel>();
    //public InterfaceViewModel InterfaceViewModel => container.Resolve<InterfaceViewModel>();


}
