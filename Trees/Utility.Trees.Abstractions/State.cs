//

namespace Utility.Trees.Abstractions
{
    [Flags]
    public enum State
    {
        Default,
        Current = 1,
        Forward = 2,
        Back = 4,
        Up = 8,
        Down = 16,
        Add = 32,
        Remove = 64
    }
}