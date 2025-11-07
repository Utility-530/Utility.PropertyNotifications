namespace Utility.Enums
{
    public enum Movement : byte
    {
        None,
        In = 1,
        Out = 2
    }

    public enum XMovement : byte
    {
        None,
        Left = 1,
        Right = 2
    }

    public enum YMovement : byte
    {
        None,
        Up = 1,
        Down = 2
    }

    public enum ZMovement : byte
    {
        None,
        Forward = 1,
        Backward = 2
    }

    public enum XYMovement : byte
    {
        None,
        Left = 1,
        Right = 2,

        Up = 4,
        Down = 8,
    }

    public enum XZMovement : byte
    {
        None,

        Left = 1,
        Right = 2,

        Forward = 16,
        Backward = 32,
    }

    public enum YZMovement : byte
    {
        None,

        Up = 4,
        Down = 8,
        Forward = 16,
        Backward = 32,
    }

    public enum XYZMovement : byte
    {
        None,

        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        Forward = 16,
        Backward = 32,
    }
}