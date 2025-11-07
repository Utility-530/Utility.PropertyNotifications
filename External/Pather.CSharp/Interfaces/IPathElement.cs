using System.Collections;

namespace Pather.CSharp.PathElements
{
    public interface IPathElement
    {
        object Apply(object target);

        void Apply(object target, object value);

        IEnumerable Apply(CSharp.Selection target);
    }
}