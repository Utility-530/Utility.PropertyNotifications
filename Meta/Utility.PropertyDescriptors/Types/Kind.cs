namespace Utility.PropertyDescriptors.Types
{
    public enum Kind
    {
        Property,
        Method,
    }

    public interface IKind
    {
        Kind Kind { get; }
    }
}