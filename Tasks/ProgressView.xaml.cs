using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace Utility.Progressions.View
{
    /// <summary>
    /// Interaction logic for ProgressView.xaml
    /// </summary>
    public partial class ProgressView
    {
        public ProgressView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                MainProgressBar.Value = ViewModel.Progress * 100;
                ProgressTextBlock.Text= ViewModel.Progress * 100 + " %";
                MainProgressBar.IsIndeterminate = ViewModel.IsIndeterminate;
            });
        }
    }
}
