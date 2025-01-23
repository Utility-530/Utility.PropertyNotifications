using Utility.Common.Models;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models.Filters;

public abstract class Filter : IPredicate, IEquatable
{
    protected Filter(string header)
    {
        Header = header;
    }

    public string Header { get; }

    public abstract bool Evaluate(object value);

    public bool Equals(IEquatable? other)
    {
        return (other as Filter)?.Header.Equals(this.Header) == true;
    }

    public string Key => Header;

    public abstract object Value { get; }
}
