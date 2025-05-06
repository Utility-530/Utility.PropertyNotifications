namespace Utility.WPF.TreeView.Connectors
{
    [Flags]
    public enum Position
    {
        Left = 1, Top = 2, Right = 4, Bottom = 8, All = Left | Top | Right | Bottom
    }
}
