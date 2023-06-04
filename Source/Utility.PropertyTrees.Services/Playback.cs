using System.Collections;
using System.Windows.Input;
using Utility.Commands;
using Utility.Enums;
using Utility.Infrastructure;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.PropertyTrees.Services;

public class Playback : BaseObject
{
    private PlaybackViewModel model = new();

    public override Key Key => new(Guids.Playback, nameof(Playback), typeof(Playback));
    public override object? Model => model;

    public IEnumerable<IObserver> Observers => throw new NotImplementedException();

    public Playback()
    {
        model.Timer.Elapsed += Timer_Elapsed;

        model.Command = new Command<Step>(step =>
        {
            ToPlayback(step);
        });
        void ToPlayback(Step step)
        {
            switch (step)
            {
                case Step.None:
                    break;
                case Step.Backward:
                    back();
                    break;
                case Step.Forward:
                    forward();
                    break;
                case Step.Walk:
                    play();
                    break;
                case Step.Wait:
                    pause();
                    //OnNext(default);
                    break;
                case Step.All:
                    break;
            }
        }
    }

    private void back()
    {
        OnNext(new BackPlaybackEvent());
    }

    private void forward()
    {
        OnNext(new ForwardPlaybackEvent());
    }

    private void pause()
    {
        model.Timer.Stop();
        //broadcast(new PausePlaybackEvent());
    }

    private void play()
    {
        model.Timer.Start();
        //broadcast(new PlayPlaybackEvent());
        model.Enabled = Step.Wait;
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        Dispatch(() => forward());
    }


    public void OnNext(ChangeSet changeSet)
    {
        if (changeSet.Any() == false)
            pause();
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

public class PlaybackViewModel : BaseViewModel
{
    private Step enabled;

    //List<Direction> directions = new();
    public System.Timers.Timer Timer { get; set; } = new(TimeSpan.FromSeconds(0.01));


    public ICommand Command { get; set; }

    public Step Enabled { get => enabled; set => Set(ref enabled, value); }
}

