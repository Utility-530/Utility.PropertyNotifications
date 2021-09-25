using System.Text;

namespace UtilityInterface.Generic.Database
{

    public interface ITimeValue<T> : NonGeneric.ITime, IValue<T>
    {
    }

}
