using Splat;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Enums;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Models;
using Utility.Nodes;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Services;

namespace Utility.WPF.Demo.Buttons
{
    public partial class PlayBackUserControl : UserControl
    {


        Playback defaultValue = Playback.Play;

        public PlayBackUserControl()
        {
            InitializeComponent();
            PlaybackService service = Locator.Current.GetService<PlaybackService>();
            PlayBackViewModel viewmodel = (PlayBackViewModel)Locator.Current.GetService<IPlaybackEngine>();
            this.DataContext = viewmodel;

            service
                .Subscribe(a =>
                {
                    if (viewmodel.Index >= 0)
                        viewmodel.Collection[viewmodel.Index].IsSelected = false;
                    if (a == ZMovement.Forward)
                    {
                        viewmodel.Index++;
                    }
                    if (a == ZMovement.Backward)
                    {
                        viewmodel.Index--;
                    }
                    if(viewmodel.Collection.Count<= viewmodel.Index)
                    {

                    }
                    var item = viewmodel.Collection[viewmodel.Index];

                    //viewmodel.Index %= viewmodel.Collection.Count;
                    item.IsSelected = true;
                    if (item.Data is MethodAction methodAction)
                    {
                        if (a == ZMovement.Forward)
                        {
                            methodAction.Do();

                        }
                        if (a == ZMovement.Backward)
                        {
                            methodAction.Undo();
                        }
                    }
                });

            Observable.Interval(TimeSpan.FromSeconds(0.1), DispatcherScheduler.Current)      
                .Select(a =>
                {
                    Debug.WriteLine($"{a} {viewmodel.Collection.Count} {viewmodel.Index}");
                    if (viewmodel.Collection.Count == 0)
                    {
                        return Playback.Pause;
                    }
                    else if (viewmodel.Collection.Count == viewmodel.Index + 1)
                    {
                        return Playback.Pause | Playback.Backward;
                    }
                    else
                    {
                        return ((Playback)defaultValue == Playback.Play ? Playback.Play : Playback.Pause | Playback.Backward | Playback.Forward);
                    }
                }).Subscribe(a =>
                {
                    Debug.WriteLine($"{a}");
                    viewmodel.Enabled = a;
                    if (EnumHelper.SeparateFlags(a).Contains(Playback.Play))
                        service.OnNext(Playback.Play);
                    else if (EnumHelper.SeparateFlags(a).Contains(Playback.Pause))
                        service.OnNext(Playback.Pause);
                });
        }
    }

    public class PlayBackViewModel : NotifyPropertyClass, IPlaybackEngine
    {
        private Enum last, enabled;

        public PlayBackViewModel()
        {
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IAction value)
        {
           
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Collection.Add(new ViewModelTree { Data = value });
            }));
        }

        public Enum Last
        {
            get => last;
            set
            {
                last = value;
                RaisePropertyChanged();
            }
        }

        public Enum Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ViewModelTree> Collection { get; } = [];
        public ObservableCollection<ViewModelTree> PastCollection { get; } = [];
        public int Index { get; set; } = -1;

    }
}
