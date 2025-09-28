using Chronic;
using IKriv.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Utility.Enums;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;


namespace Utility.Services
{
    public class PlaybackService : IObservable<ZMovement>, IObserver<Playback>, IGetValue<Playback>
    {
        ReplaySubject<ZMovement> movement = new(1);

        public Playback Value { get; set; } = Playback.Pause;
        object IGetValue.Value => Value;
        public PlaybackService()
        {
        }

        public IDisposable Subscribe(IObserver<ZMovement> observer)
        {
            return movement.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Playback value)
        {
            switch (Value = value)
            {
                case Playback.Play:
                    {
                        movement.OnNext(ZMovement.Forward);
                        break;
                    }
                case Playback.Forward:
                    {
                        movement.OnNext(ZMovement.Forward);
                        Value = Playback.Pause;
                        break;
                    }
                case Playback.Backward:
                    {
                        movement.OnNext(ZMovement.Backward);
                        Value = Playback.Pause;
                        break;
                    }
                case Playback.Pause:
                    {
                        Value = Playback.Pause;
                        break;
                    }
            }
        }
    }
}