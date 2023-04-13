using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

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

    public class BeatViewModel : ReactiveObject
    {
        private double rate = 1d;
        private ObservableAsPropertyHelper<long> beat;

        public double Rate { get => rate; set => this.RaiseAndSetIfChanged(ref rate, value); }

        public long Beat => beat.Value;

        public BeatViewModel()
        {
            beat = this.WhenAnyValue(a => a.Rate).Where(r => r > 0).Select(r => Observable.Interval(TimeSpan.FromSeconds(1d / r))).Switch().ToProperty(this, a => a.Beat);
        }
    }
}