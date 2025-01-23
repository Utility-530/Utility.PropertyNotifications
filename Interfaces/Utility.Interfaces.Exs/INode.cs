using Utility.Trees.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Enums;
using System.Collections;

namespace Utility.Interfaces.Exs
{
    public interface INode : ITree, IIsExpanded, ICurrent<INode>, IAdd, IKey, ILocalIndex, IIsPersistable, IIsEditable, IOrientation, IRemoved
    {
    }

    public interface ICurrent<T>
    {
        T Current { get; set; }
    }

    public interface ILocalIndex
    {
        int? LocalIndex { get; set; }
    }

    public interface IOrientation
    {
        Orientation Orientation { get; set; }
    }
        
    public interface IProliferation
    {
        IEnumerable Proliferation();
    }

    public interface ISetNode
    {
        void SetNode(INode node);

    }
}
