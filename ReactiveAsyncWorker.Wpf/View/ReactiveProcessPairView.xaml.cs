using ReactiveUI;
using System.Linq;
using System.Reactive.Disposables;

namespace ReactiveAsyncWorker.Wpf.View
{
    /// <summary>
    /// Interaction logic for MultiTaskView.xaml
    /// </summary>
    public partial class ReactiveProcessPairView
    {
        public ReactiveProcessPairView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                KeyTextBlock.Text = ViewModel.Key.ToString();


                this.OneWayBind(this.ViewModel, vm => vm.Value, v => v.ValueTextBlock.Text, a => a.Count().ToString())
                        .DisposeWith(disposable);
            });
        }
    }
}
