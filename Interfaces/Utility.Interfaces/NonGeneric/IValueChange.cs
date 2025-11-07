namespace Utility.Interfaces.NonGeneric
{
    public interface IValueChange : IName
    {
        object NewValue { get; }

        object OldValue { get; }
    }
}