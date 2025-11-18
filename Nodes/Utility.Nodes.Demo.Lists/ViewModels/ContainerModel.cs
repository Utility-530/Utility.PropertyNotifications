//using Dragablz;
//using Dragablz.Controls;
//using Splat;
//using Utility.Interfaces.Exs;
//using Utility.Models;

//namespace Utility.Nodes.Demo.Lists
//{
//    public class ContainerModel : Model
//    {
//        public ContainerModel()
//        {
//            //_cleanUp = Disposable.Create(() =>
//            //{
//            //    //menuController.Dispose();
//            //    //foreach (var disposable in Views.Select(vc => vc.Content).OfType<IDisposable>())
//            //    //    disposable.Dispose();
//            //});
//        }

//        //public ItemActionCallback ClosingTabItemHandler => ClosingTabItemHandlerImpl;

//        //private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs args)
//        //{
//        //    var container = args.DragablzItem.DataContext;//.DataContext;
//        //    if (container?.Equals(Current) == true)
//        //    {
//        //        Current = m_items.Cast<IViewModelTree>().FirstOrDefault(vc => vc != container);
//        //        RaisePropertyChanged(nameof(Current));
//        //    }
//        //    //var disposable = container.Content as IDisposable;
//        //    //disposable?.Dispose();
//        //}

//        //public IInterTabClient InterTabClient => Locator.Current.GetService<IInterTabClient>();
//    }
//}