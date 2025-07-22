using System;

namespace Utility.Enums
{
    [Flags]
    public enum Playback
    {
        Pause = 0, 
        Backward = 1, 
        Forward = 2, 
        Play = 4
    }
}