namespace Utility.Enums
{
    public enum Movement : byte
    {
        None,
        InToOut = 1,
        OutToIn = 2
    }

    public enum XMovement : byte
    {
        None,
        LeftToRight = 1,
        RightToLeft = 2
    }  
    
    public enum YMovement : byte
    {
        None,
        TopToBottom = 1,
        BottomToTop = 2
    }   
    
    public enum ZMovement : byte
    {
        None,
        FrontToBack = 1,
        BackToFront = 2
    }


    public enum XYMovement : byte
    {
        None,
        LeftToRight = 1,
        RightToLeft = 2,

        TopToBottom = 4,
        BottomToTop = 8,


    }

    public enum XZMovement : byte
    {
        None,

        LeftToRight = 1,
        RightToLeft = 2,

        FrontToBack = 16,
        BackToFront = 32,
    }
    public enum YZMovement : byte
    {
        None,


        TopToBottom = 4,
        BottomToTop = 8,
        FrontToBack = 16,
        BackToFront = 32,
    }
    public enum XYZMovement : byte
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