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
    /// Interaction logic for FactoryView.xaml
    /// </summary>
    public partial class FactoryView 
    {
        public FactoryView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {

                this.OneWayBind(this.ViewModel, a => a.CreatedCollection, v => v.CreatedItemsControl.ItemsSource)
                .DisposeWith(disposable);            
                
                this.OneWayBind(this.ViewModel, a => a.ScheduledCollection, v => v.ScheduledListBox.ItemsSource)
                .DisposeWith(disposable);

                CombinedItemsControl.ItemsSource = this.ViewModel.CombinedCollection;

            });
        }
    }
}
