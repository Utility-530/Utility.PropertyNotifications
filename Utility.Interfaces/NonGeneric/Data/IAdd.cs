using System.Collections;

namespace Utility.Interfaces.NonGeneric.Data
{
    public interface IAdd
    {
        object Add(object item);
    }

    public interface IAddBy
    {
        object AddBy(IQuery query);
    }

    public interface IAddMany
    {
        IEnumerable AddMany(IEnumerable items);
    }

    public interface IAddManyBy
    {
        IEnumerable AddManyBy(IQuery query);
    }
}