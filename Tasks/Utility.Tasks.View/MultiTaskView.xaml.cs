using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Utility.Tasks.View
{
    /// <summary>
    /// Interaction logic for MultiTaskView.xaml
    /// </summary>
    public partial class MultiTaskView
    {
        public MultiTaskView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                ViewModelsViewHost.ViewModel = ViewModel.ViewModels;

                ReadyViewModelViewHost.ViewModel = ViewModel.ReadyViewModel;

                CreatedViewModelsViewHost.ViewModel = ViewModel.CreatedViewModel;

                RunningViewModelViewHost.ViewModel = ViewModel.RunningViewModel;

                TerminatedViewModelViewHost.ViewModel = ViewModel.TerminatedViewModel;

                CombinedViewModelViewHost.ViewModel = ViewModel.CombinedCollection;

                GroupedViewModelViewHost.ViewModel = ViewModel.GroupedCollection;

                LatestViewModelViewHost.ViewModel = ViewModel.LatestCollection;

                SwitchViewToggleButton.Events().Checked.Select(a => true)
                .Merge(SwitchViewToggleButton.Events().Unchecked.Select(a => false))
                .StartWith(default(bool))
                .Subscribe(a =>
                {
                    CombinedViewModelViewHost.Visibility = a ? Visibility.Collapsed : Visibility.Visible;
                    GroupedViewModelViewHost.Visibility = a ? Visibility.Visible : Visibility.Collapsed;
                });

                SwitchView2ToggleButton.Events().Checked.Select(a => true)
                .Merge(SwitchView2ToggleButton.Events().Unchecked.Select(a => false))
                .StartWith(default(bool))
                .Subscribe(a =>
                {
                    LatestViewModelViewHost.Visibility = a ?  Visibility.Visible: Visibility.Collapsed;       
                });
            });
        }
    }
}
