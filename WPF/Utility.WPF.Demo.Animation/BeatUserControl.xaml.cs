using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.PropertyNotifications;

namespace Utility.WPF.Demo.Animation
{
    /// <summary>
    /// Interaction logic for BeatUserControl.xaml
    /// </summary>
    public partial class BeatUserControl : UserControl
    {
        public BeatUserControl()
        {
            InitializeComponent();
        }
    }

    public class BeatViewModel : NotifyPropertyClass
    {
        private double rate = 1d;
        private long beat;

        public double Rate { get => rate; set => this.RaisePropertyChanged(ref rate, value); }

        public long Beat { get => beat; set => this.RaisePropertyChanged(ref beat, value); }

        public BeatViewModel()
        {
            this.WithChangesTo(a => a.Rate).Where(r => r > 0).Select(r => Observable.Interval(TimeSpan.FromSeconds(1d / r)))
                .Switch()
                .Subscribe(a => Beat = a);
        }
    }
}