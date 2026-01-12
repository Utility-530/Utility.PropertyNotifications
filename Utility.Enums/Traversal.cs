namespace Utility.Enums
{
    public enum Traversal : byte
    {
        None,
        InToOut = 1,
        OutToIn = 2
    }

    public enum XTraversal : byte
    {
        None,
        LeftToRight = 1,
        RightToLeft = 2
    }

    public enum YTraversal : byte
    {
        None,
        TopToBottom = 1,
        BottomToTop = 2
    }

    public enum ZTraversal : byte
    {
        None,
        FrontToBack = 1,
        BackToFront = 2
    }

    public enum XYTraversal : byte
    {
        None,
        LeftToRight = 1,
        RightToLeft = 2,

        TopToBottom = 4,
        BottomToTop = 8,
    }

    public enum XZTraversal : byte
    {
        None,

        LeftToRight = 1,
        RightToLeft = 2,

        FrontToBack = 16,
        BackToFront = 32,
    }

    public enum YZTraversal : byte
    {
        None,

        TopToBottom = 4,
        BottomToTop = 8,
        FrontToBack = 16,
        BackToFront = 32,
    }

    public enum XYZTraversal : byte
    {
        None,

        LeftToRight = 1,
        RightToLeft = 2,
        TopToBottom = 4,
        BottomToTop = 8,
        FrontToBack = 16,
        BackToFront = 32,
    }
}