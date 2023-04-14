namespace Utility.Models
{
    public interface ISerialise
    {
        string ToString();

        ISerialise FromString(string str);
    }
}