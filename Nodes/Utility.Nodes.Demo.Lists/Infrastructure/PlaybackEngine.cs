using Utility.Interfaces.Exs;

namespace Utility.Nodes.Demo.Lists
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
