//using DryIoc;
//using MoreLinq;
//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading.Tasks;
//using Utility.Nodes;
//using Utility.Nodify.Base.Abstractions;
//using Utility.Nodify.Core;
//using IContainer = DryIoc.IContainer;

//namespace Utility.Nodify.ViewModels;
//using Keys = Utility.Nodify.Operations.Keys;

//public class MainViewModel
//{

//    private readonly IContainer container;


//    public MainViewModel(IContainer container)
//    {
//        this.container = container;
//        TabsViewModel.AddEditorCommand.Execute(container.Resolve<DiagramsViewModel>());

//        var selected = container.Resolve<RangeObservableCollection<Diagram>>(serviceKey: Keys.SelectedDiagram);
//        selected.CollectionChanged += MainViewModel_CollectionChanged;
//        if (selected.FirstOrDefault() is Diagram item)
//        {
//            //var diagramViewModel = container.Resolve<DiagramViewModel>();
//            try
//            {
//                var diagramViewModel = container.Resolve<IConverter>().Convert(item);

//                container.RegisterInstance(diagramViewModel);
//                TabsViewModel.AddEditorCommand.Execute(diagramViewModel);
//            }
//            catch (Exception ex)
//            {

//            }
//        }
//    }



//    private void MainViewModel_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
//    {
//        //foreach (Diagram item in e.NewItems)
//        //{
//        TabsViewModel.AddEditorCommand.Execute(container.Resolve<DiagramViewModel>());
//        // }
//    }

//    public MessagesViewModel MessagesViewModel => container.Resolve<MessagesViewModel>();
//    public TabsViewModel TabsViewModel => container.Resolve<TabsViewModel>();
//    //public InterfaceViewModel InterfaceViewModel => container.Resolve<InterfaceViewModel>();


//}
