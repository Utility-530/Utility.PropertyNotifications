using Utility.Enums;
using Utility.Models;
using playback = Utility.Enums.Playback;

namespace Utility.Infrastructure
{
    public class Playback : BaseObject //, IPlayback
    {
        List<Direction> directions = new();
        public System.Timers.Timer Timer { get; set; } = new(TimeSpan.FromSeconds(0.01));


        public override Key Key => Keys.Playback;

        public Playback()
        {
            Timer.Elapsed += Timer_Elapsed;
        }

        public void Back()
        {
            Broadcast(Direction.Backward);
        }

        public void Forward()
        {
            Broadcast(Direction.Forward);
        }

        public void Pause()
        {
            Timer.Stop();

        }

        public void Play()
        {
            Timer.Start();
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Forward();
        }


        public override void OnNext(object value)
        {
            if (value is playback playback)
            {
                switch (playback)
                {
                    case playback.Pause:
                        Pause();
                        return;
                    case playback.Play:
                        Play();
                        return;
                    case playback.Forward:
                        Forward();
                        return;
                    case playback.Back:
                        Back();
                        return;
                }
            }
        }
    }
}
