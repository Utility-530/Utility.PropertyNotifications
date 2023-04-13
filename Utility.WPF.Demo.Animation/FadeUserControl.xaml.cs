using System;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Animation
{
    /// <summary>
    /// Interaction logic for FadeUserControl.xaml
    /// </summary>
    public partial class FadeUserControl : UserControl
    {
        public FadeUserControl()
        {
            InitializeComponent();



            var progress = Observable
                                                        .Interval(TimeSpan.FromSeconds(0.1))

                                                        .Select(a => (a * 0.05 * 100) % 101)

                .ObserveOnDispatcher()
                .Subscribe(a =>
                {
                    MainProgressBar.Value = a;
                    if ((int)a == 100)
                    {
                        MainFadeControl.FadeCommand.Execute(default);
                    }
                });


            var interval = Observable
                .Interval(TimeSpan.FromSeconds(3));


            interval.ObserveOnDispatcher().Subscribe(a =>
            {
                FadeText.FadeCommand.Execute(default);
            });
        }
    }
}