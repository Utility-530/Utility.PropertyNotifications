using Dragablz;
using DynamicData;
using DynamicData.Binding;
using Splat;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Windows.Input;
using Utility.Commands;
using Utility.Nodes.WPF;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Lists
{
    public class ContainerViewModel : NotifyPropertyClass
    {
        private readonly Command _showMenuCommand;
        private readonly IDisposable _cleanUp;
        private ViewModel _selected;

        public ICommand MemoryCollectCommand { get; } = new Command(() =>
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        });


        public ContainerViewModel()
        {
            Locator.Current
                .GetService<IObservable<ViewModel>>()
                .Subscribe(a =>
                {  
                    Selected = a;
                    Views.Add(a);
                });
            //var menuController = Views.ToObservableChangeSet()
            //                            .Filter(vc => vc is MasterViewModel)
            //                            .Transform(vc =>
            //                            {
            //                                var x = (vc as MasterViewModel).Selection;
            //                                foreach (var item in x)
            //                                {
            //                                    item.WhenAnyPropertyChanged().Subscribe(x =>
            //                                    {
            //                                        var content = _objectProvider.Get(item.Type);
            //                                        var _vc = new ViewContainer(item.Title, content);
            //                                        Views.Add(_vc);
            //                                        Selected = _vc;

            //                                    });
            //                                }
            //                                return x;
            //                            })
            //                            .Subscribe(item =>
            //                            {

            //                            });

            var existing = Views.FirstOrDefault(vc => vc is MasterViewModel);
            if (existing == null)
            {
                var newItem = Locator.Current.GetService<MasterViewModel>();
                newItem.IsSelected = true;
                Views.Add(newItem);
                Selected = newItem;
            }
            else
            {
                Selected = existing;
            }

            _cleanUp = Disposable.Create(() =>
            {
                //menuController.Dispose();
                //foreach (var disposable in Views.Select(vc => vc.Content).OfType<IDisposable>())
                //    disposable.Dispose();
            });
        }


        public ObservableCollection<ViewModel> Views { get; } = new ObservableCollection<ViewModel>();
        public ItemActionCallback ClosingTabItemHandler => ClosingTabItemHandlerImpl;

        private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            var container = (ViewModel)args.DragablzItem.DataContext;//.DataContext;
            if (container?.Equals(Selected) == true)
            {
                Selected = Views.FirstOrDefault(vc => vc != container);
            }
            //var disposable = container.Content as IDisposable;
            //disposable?.Dispose();
        }
        public ViewModel Selected
        {
            get => _selected;
            set
            {
                foreach (var v in Views)
                    v.IsSelected = false;
                value.IsSelected = true;
                RaisePropertyChanged(ref _selected, value);
            }
        }

        public IInterTabClient InterTabClient => Locator.Current.GetService<IInterTabClient>();


        public void Dispose()
        {
            _cleanUp.Dispose();
        }



    }
}
