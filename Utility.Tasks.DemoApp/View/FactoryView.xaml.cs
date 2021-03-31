using ReactiveUI;
using System.Reactive.Disposables;

namespace Utility.Tasks.DemoApp.View
{
    /// <summary>
    /// Interaction logic for FactoryView.xaml
    /// </summary>
    public partial class FactoryView
    {
        public FactoryView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.BindCommand(this.ViewModel, a => a.Configuration.Create, a => a.StandardButton)
                    .DisposeWith(disposable);

                this.BindCommand(this.ViewModel, a => a.Configuration.CreateDelayed, a => a.DelayedButton)
                    .DisposeWith(disposable);

                this.OneWayBind(this.ViewModel, a=>a.Main, a=>a.MainViewModelViewHost.ViewModel)
                    .DisposeWith(disposable);
            });
        }
    }
}
