namespace Utility.Enums
{
    /// <summary>
    /// <a href="https://en.wikipedia.org/wiki/Relative_direction"></a>
    /// relative direction
    /// </summary>
    [System.Flags]
    public enum Direction : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
        Front = 4,
        Back = 8,
        Up = 16,
        Down = 32,
        All = Left | Right | Front | Back | Up | Down
    }

    [System.Flags]
    public enum XDirection : byte
    {
        None = 0,
        Left = 1,
        Right = 2,

        All = Left | Right
    }

    [System.Flags]
    public enum YDirection : byte
    {
        None = 0,

        Up = 16,
        Down = 32,
        All = Up | Down
    }

    [System.Flags]
    public enum ZDirection : byte
    {
        None = 0,
        Front = 4,
        Back = 8,
        All = Front | Back
    }

    [System.Flags]
    public enum XYDirection : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 16,
        Down = 32,
        All = Left | Right | Up | Down
    }

    [System.Flags]
    public enum YZDirection : byte
    {
        None = 0,
        Front = 4,
        Back = 8,
        Up = 16,
        Down = 32,
        All = Front | Back | Up | Down
    }

    [System.Flags]
    public enum XZDirection : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
        Front = 4,
        Back = 8,
        All = Left | Right | Front | Back
    }
}