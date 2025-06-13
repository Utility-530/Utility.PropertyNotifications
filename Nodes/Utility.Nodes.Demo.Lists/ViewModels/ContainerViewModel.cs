using Dragablz;
using Splat;
using System.Collections.ObjectModel;
using Utility.Nodes.WPF;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists
{
    public class ContainerViewModel : NotifyPropertyClass
    {
        private ViewModel _selected;

        public ContainerViewModel()
        {
            //_cleanUp = Disposable.Create(() =>
            //{
            //    //menuController.Dispose();
            //    //foreach (var disposable in Views.Select(vc => vc.Content).OfType<IDisposable>())
            //    //    disposable.Dispose();
            //});
        }

        public ObservableCollection<ViewModel> Children { get; } = new ObservableCollection<ViewModel>();

        public ItemActionCallback ClosingTabItemHandler => ClosingTabItemHandlerImpl;

        private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            var container = (ViewModel)args.DragablzItem.DataContext;//.DataContext;
            if (container?.Equals(Selected) == true)
            {
                Selected = Children.FirstOrDefault(vc => vc != container);
            }
            //var disposable = container.Content as IDisposable;
            //disposable?.Dispose();
        }

        public ViewModel Selected
        {
            get => _selected;
            set
            {
                foreach (var v in Children)
                    v.IsSelected = false;
                value.IsSelected = true;
                RaisePropertyChanged(ref _selected, value);
            }
        }

        public IInterTabClient InterTabClient => Locator.Current.GetService<IInterTabClient>();


    }
}
