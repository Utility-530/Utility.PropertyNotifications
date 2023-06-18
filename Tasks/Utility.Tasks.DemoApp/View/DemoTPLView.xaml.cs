using ReactiveUI;
using System.Reactive.Disposables;

namespace Utility.Tasks.DemoApp.View
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
                MainViewModelViewHost.ViewModel = ViewModel.MainViewModel.Service;

                CollectionModelViewHost.ViewModel = ViewModel.MainViewModel.Collection;

                LoginDialogViewModelViewHost.ViewModel = ViewModel.LoginDialogViewModel;

                OutputItemsControl.ItemsSource = ViewModel.OutputModel.Collection;

                this.BindCommand(this.ViewModel, vm => vm.ConfigurationViewModel.StartCommand, v => v.StartButton).DisposeWith(disposable);

                this.BindCommand(this.ViewModel, vm => vm.ConfigurationViewModel.StartManyCommand, v => v.StartManyButton).DisposeWith(disposable);

                MainOversizedNumberSpinner
                    .WhenAnyValue(a => a.Value)
                    .InvokeCommand(this.ViewModel.ConfigurationViewModel, a => a.CapacityCommand)
                    .DisposeWith(disposable);



                ProgressViewModelViewHost.ViewModel = ViewModel.ProgressViewModel;
            });

        }
    }
}
