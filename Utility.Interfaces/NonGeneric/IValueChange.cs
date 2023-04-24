namespace Utility.Interfaces.NonGeneric
{
    public interface IValueChange : IName, IEquatable
    {
        object NewValue { get; }

        object OldValue { get; }
    }
}
