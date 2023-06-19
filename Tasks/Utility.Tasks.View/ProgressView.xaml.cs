using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility.Enums;

namespace Utility.Tasks.View
{
    /// <summary>
    /// Interaction logic for ProgressStateView.xaml
    /// </summary>
    public partial class ProgressView
    {
        public ProgressView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                //var storyBoard = this.Resources["OpacityStoryboard"] as Storyboard;

                this.BindCommand(this.ViewModel, vm => vm.Command, v => v.MainButton).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.HasPotentialProgress, v => v.MainButton.IsEnabled, a => a).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.State, v => v.MainButton.Content, a => {
                    //storyBoard.Begin();
                    return a switch
                    {
                        ProcessState.Ready => new PackIcon { Height = 20, Width = 20, Kind = PackIconKind.Cancel},
                        ProcessState.Running => new PackIcon { Height = 20, Width = 20, Kind = PackIconKind.Cancel },
                        ProcessState.Created => new PackIcon { Height = 20, Width = 20, Kind = PackIconKind.Play },
                        ProcessState.Terminated => new PackIcon { Height = 20, Width = 20, Kind = PackIconKind.Check, IsEnabled = false },
                    };
                }).DisposeWith(disposable);


                KeyTextBlock.Text = ViewModel.Key;
                DateTextBlock.Text = ViewModel.Progress.Started.ToString("u");
                SummaryItemsControl.ItemsSource = ViewModel.Collection;
                this.OneWayBind(this.ViewModel, vm => vm.Progress.IsIndeterminate, v => v.MainProgressBar.IsIndeterminate).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.Progress.Value, v => v.MainProgressBar.Value, a=> a * 100d).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.State, v => v.StateTextBlock.Text).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, vm => vm.State, v => v.MainProgressBar.Visibility, a => a switch
                {
                    ProcessState.Terminated => Visibility.Visible,
                    ProcessState.Running => Visibility.Visible,
                    ProcessState.Ready => Visibility.Visible,
                    _ => Visibility.Collapsed
                }).DisposeWith(disposable);            
            });


        }
    }
}
