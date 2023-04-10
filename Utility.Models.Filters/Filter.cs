using Utility.Common.Models;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models.Filters;

public abstract class Filter : Model, IPredicate, IKey
{
    protected Filter(string header)
    {
        Header = header;
    }

    public string Header { get; }

    public abstract bool Invoke(object value);

    public string Key => Header;

    public abstract object Value { get;  }
}
