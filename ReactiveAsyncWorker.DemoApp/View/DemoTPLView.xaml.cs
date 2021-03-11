using ReactiveUI;
using Splat;
using System;
using System.Reactive.Disposables;
using System.Windows.Controls;


namespace ReactiveAsyncWorker.DemoApp.View
{
    /// <summary>
    /// Interaction logic for TPLViewModel.xaml
    /// </summary>
    public partial class DemoTPLView
    {
        public DemoTPLView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                //var masterTPLViewModel = Locator.Current.GetService<MasterTPLViewModel>();

                MainViewModelViewHost.ViewModel = ViewModel.MainViewModel.Service;

                CollectionModelViewHost.ViewModel = ViewModel.MainViewModel.Collection;

                ItemsControl1.ItemsSource = ViewModel.OutputModel.Collection;

                this.DataContext = ViewModel;

                //this.OneWayBind(this.ViewModel, vm=>vm., v=>v.MainOversizedNumberSpinner.Value).DisposeWith(disposable);

                MainOversizedNumberSpinner
                    .WhenAnyValue(a => a.Value)
                    .InvokeCommand(this.ViewModel.ConfigurationViewModel, a => a.CapacityCommand)
                    .DisposeWith(disposable);
   
            });

        }
    }
}
