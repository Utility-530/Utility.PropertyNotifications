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
    public partial class MultiProgressView
    {
        public MultiProgressView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.MainItemsControl.ItemsSource = ViewModel.Collection;
                this.OneWayBind(this.ViewModel, vm => vm.Current, v => v.MainPopupBox.ToggleContent, a=> {
                    return new ViewModelViewHost { 
                        ViewModel = a,
                        VerticalContentAlignment=System.Windows.VerticalAlignment.Stretch, 
                        HorizontalContentAlignment=System.Windows.HorizontalAlignment.Stretch};
                }).DisposeWith(disposable);
             
            });
        }
    }
}
