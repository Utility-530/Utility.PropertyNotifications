using System.Collections;

namespace PropertyTrees.Abstractions
{
    public interface IPropertyNode
    {
        public IEnumerable Children { get; }
    }
}