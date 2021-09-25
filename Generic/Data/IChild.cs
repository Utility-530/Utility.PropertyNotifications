using UtilityInterface.NonGeneric.Data;

namespace UtilityInterface.Generic.Data
{
    public interface IChild<T> : IChild where T : IId
    {
        T Parent { get;  }
    }

}
