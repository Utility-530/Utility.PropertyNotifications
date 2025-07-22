using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.Exs
{
    public interface IAction
    {
        void Do();
        void Undo();
    }
    public interface IPlaybackEngine:IObserver<IAction>
    {
    }
}
