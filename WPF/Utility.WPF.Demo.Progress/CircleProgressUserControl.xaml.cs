using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.WPF.Controls.Progress;

namespace Utility.WPF.Demo.Progress
{
    /// <summary>
    /// Interaction logic for GaugeTwoUserControl.xaml
    /// </summary>
    public partial class CircleProgressUserControl : UserControl
    {
        public CircleProgressUserControl()
        {
            InitializeComponent();

            Observable
                .Interval(TimeSpan.FromSeconds(0.1))
                .ObserveOn(SynchronizationContextScheduler.Instance)
                .Subscribe(a =>
                {
                    ProgressCircle.Value = a % 90;
                });
        }
    }
}