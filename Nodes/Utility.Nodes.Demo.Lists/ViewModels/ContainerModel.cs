using Dragablz;
using Splat;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Nodes.Demo.Lists
{
    public class  ContainerModel : Model
    {
        private object _selected;

        public ContainerModel()
        {
            //_cleanUp = Disposable.Create(() =>
            //{
            //    //menuController.Dispose();
            //    //foreach (var disposable in Views.Select(vc => vc.Content).OfType<IDisposable>())
            //    //    disposable.Dispose();
            //});
        }

        public ItemActionCallback ClosingTabItemHandler => ClosingTabItemHandlerImpl;

        private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            var container = args.DragablzItem.DataContext;//.DataContext;
            if (container?.Equals(Selected) == true)
            {
                Selected = m_items.Cast<object>().FirstOrDefault(vc => vc != container);
            }
            //var disposable = container.Content as IDisposable;
            //disposable?.Dispose();
        }

        public object Selected
        {
            get => _selected;
            set
            {
                foreach (var v in Children)
                {
                    if (v is ISetIsSelected setIsSelected)
                        setIsSelected.IsSelected = false;
                }
                if(value is ISetIsSelected setSelected)
                    setSelected.IsSelected = true;
                RaisePropertyChanged(ref _selected, value);
            }
        }

        public IInterTabClient InterTabClient => Locator.Current.GetService<IInterTabClient>();


    }
}
