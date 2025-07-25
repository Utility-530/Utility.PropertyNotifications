using Moq;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Enums;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Services;
using Utility.ServiceLocation;

namespace Utility.Simulation
{
    public class PlaybackEngine : IPlaybackEngine
    {
        Playback defaultValue;
        PlaybackService service => Globals.Resolver.Resolve<PlaybackService>();
        PlayBackViewModel playBackviewmodel => Globals.Resolver.Resolve<PlayBackViewModel>();
        HistoryViewModel viewmodel => Globals.Resolver.Resolve<HistoryViewModel>();

        public PlaybackEngine(Playback playback = Playback.Play)
        {
            defaultValue = playback;
            service
                .Subscribe(a =>
                {
                    if (viewmodel.Index >= 0)
                        (viewmodel.Collection[viewmodel.Index] as ISetIsSelected).IsSelected = false;
                    if (a == ZMovement.Forward)
                    {
                        viewmodel.Index++;
                    }
                    if (a == ZMovement.Backward)
                    {
                        viewmodel.Index--;
                    }
                    if (viewmodel.Collection.Count <= viewmodel.Index)
                    {

                    }
                });

            playBackviewmodel.WithChangesTo(a => a.Last)
                .Subscribe(a =>
                {
                    var playback = (Playback)a;
                    service.OnNext(playback);
                });

            viewmodel
                .WhenChanged(a => a.Index)
                .Subscribe(item =>
                {
                    var x = viewmodel.CurrentItem as ISetIsSelected;
                    var data = viewmodel.CurrentItem as IData;
                    //viewmodel.Index %= viewmodel.Collection.Count;
                    x.IsSelected = true;
                    if (data?.Data is IAction methodAction)
                    {
                        if ((int)item.Value - (int)item.PreviousValue > 0)
                        {
                            methodAction.Do();

                        }
                        else
                        {
                            methodAction.Undo();
                        }
                    }
                    else
                        throw new Exception("3433 d2sa");

                });

            //DispatcherScheduler.Current
            Observable
                .Interval(TimeSpan.FromSeconds(0.1), Globals.Resolver.Resolve<IScheduler>())
                .Select(a =>
                {
                    //Debug.WriteLine($"{a} {viewmodel.Collection.Count} {viewmodel.Index}");
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
                        return (defaultValue == Playback.Play ? Playback.Play : Playback.Pause | Playback.Backward | Playback.Forward);
                    }
                }).Subscribe(a =>
                {
                    //Debug.WriteLine($"{a}");
                    playBackviewmodel.Enabled = a;
                    if (EnumHelper.SeparateFlags(a).Contains(Playback.Play))
                        service.OnNext(Playback.Play);
                    else if (EnumHelper.SeparateFlags(a).Contains(Playback.Pause))
                        service.OnNext(Playback.Pause);
                });
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
            Utility.Globals.UI.Post(a =>
            {
                var created = Globals.Resolver.Resolve<IFactory<INode>>().Create(a);
                viewmodel.Collection.Add(created);
            }, value);
            //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{

            //}));
        }
    }
}
