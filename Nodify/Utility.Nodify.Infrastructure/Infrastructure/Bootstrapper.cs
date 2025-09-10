
using Utility.Nodify.Core;
using DryIoc;
using Utility.Nodify.ViewModels;

namespace Utility.Nodify.ViewModels.Infrastructure
{
    public class Bootstrapper
    {
        public static IContainer Build(IContainer container)
        {
            container.Register<DiagramsViewModel>();        
            //container.Register<TabsViewModel, CustomTabsViewModel>();
            container.Register<MessagesViewModel>();
           
      
            return container;
        }
    }

    //public class CustomTabsViewModel : TabsViewModel
    //{
    //    private readonly IContainer container;

    //    public CustomTabsViewModel(IContainer container)
    //    {
    //        this.container = container;
    //    }

    //    protected override object Content
    //    {
    //        get
    //        {
    //            var diagram = new Diagram();
    //            //var viewmodel = container.Resolve<IConverter>().Convert(diagram);
    //            return viewmodel;
    //        }
    //    }
    //}
}
