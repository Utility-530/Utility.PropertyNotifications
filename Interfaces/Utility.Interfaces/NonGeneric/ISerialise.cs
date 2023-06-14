namespace Utility.Interfaces.NonGeneric
{
    public interface ISerialise
    {
        string ToString();

        ISerialise FromString(string str);
    }
}