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
        Forward = 4,
        Backward = 8,
        Up = 16,
        Down = 32,
        All = Left | Right | Forward | Backward | Up | Down
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
        Forward = 4,
        Backward = 8,
        All = Forward | Backward
    }


    [System.Flags]
    public enum XYDirection : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 16,
        Down = 32,
        All = Left | Right  | Up | Down
    }

    [System.Flags]
    public enum YZDirection : byte
    {
        None = 0,
        Forward = 4,
        Backward = 8,
        Up = 16,
        Down = 32,
        All =  Forward | Backward | Up | Down
    }

    [System.Flags]
    public enum XZDirection : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
        Forward = 4,
        Backward = 8,
        All = Left | Right | Forward | Backward 
    }
}