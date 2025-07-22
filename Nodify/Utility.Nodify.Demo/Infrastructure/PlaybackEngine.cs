using DryIoc.ImTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;

namespace Utility.Nodify.Demo.Infrastructure
{
    public class PlaybackEngine : IPlaybackEngine
    {
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
            value.Do();
        }
    }
}
