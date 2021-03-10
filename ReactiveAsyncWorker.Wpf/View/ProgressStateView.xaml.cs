using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReactiveAsyncWorker.Wpf.View
{
    /// <summary>
    /// Interaction logic for ProgressStateView.xaml
    /// </summary>
    public partial class ProgressStateView 
    {
        public ProgressStateView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(this.ViewModel, vm => vm.Key, v => v.KeyTextBlock.Text).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.Date, v => v.DateTextBlock.Text, a => a.ToString("u")).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.IsIndeterminate, v => v.MainProgressBar.IsIndeterminate).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.Progress, v => v.MainProgressBar.Value).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.State, v => v.StateTextBlock.Text).DisposeWith(disposable);

            });
        }
    }
}
