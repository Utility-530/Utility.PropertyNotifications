using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Utility.Enums;
using Utility.Nodes;
using Utility.WPF.Behaviors;
using Utility.WPF.Demo.Data.Factory;
using Utility.Helpers.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.WPF.Demo.Buttons
{
    public partial class PlayBackUserControl : UserControl
    {
        Utility.Services.PlaybackService service = new();
        PlayBackViewModel viewmodel = new PlayBackViewModel();
        public PlayBackUserControl()
        {
            InitializeComponent();
            this.DataContext = viewmodel;
            service
                .Subscribe(a =>
                {
                    var selected = viewmodel.Collection[viewmodel.Index].IsSelected = false;
                    if (a == ZMovement.Forward)
                    {
                        viewmodel.Index++;
                    }
                    if (a == ZMovement.Backward)
                    {
                        viewmodel.Index--;
                    }
                    viewmodel.Index %= viewmodel.Collection.Length;
                    viewmodel.Collection[viewmodel.Index].IsSelected = true;

                });

            viewmodel
                .WithChangesTo(a => a.Last)
                .Subscribe(a =>
                {
                    if(a is Playback playback)
                    {
                        service.OnNext(playback);
                    }
                });
        }
    }

    public class PlayBackViewModel : NotifyPropertyClass
    {
        private Enum last;

        public PlayBackViewModel()
        {
            Collection = [.. ProfileFactory.BuildPool().Select(a => new ViewModelTree { Data = a })];
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

        public ViewModelTree[] Collection { get; } = [];
        public int Index { get; set; } = 0;

    }
}
