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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilityEnum;

namespace Utility.Tasks.View
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
                KeyTextBlock.Text = ViewModel.Key;
                DateTextBlock.Text = ViewModel.Progress.Started.ToString("u");
                MainProgressBar.IsIndeterminate = ViewModel.Progress.IsIndeterminate;
                MainProgressBar.Value = ViewModel.Progress.Value * 100d;
                StateTextBlock.Text = ViewModel.State.ToString();
                OutputTextBlock.Text = JsonConvert.SerializeObject(ViewModel.Output);

                this.MainProgressBar.Visibility = ViewModel.State switch
                {
                    ProcessState.Terminated => Visibility.Visible,
                    ProcessState.Running => Visibility.Visible,
                    _ => Visibility.Collapsed
                };
            });
        }
    }
}
