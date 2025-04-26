using Utility.Trees.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Enums;
using System.Collections;
using System.Windows.Input;

namespace Utility.Interfaces.Exs
{
    public interface INode : ITree, IIsExpanded, ICurrent<INode>, IAdd, IKey, ILocalIndex, IIsPersistable, IIsEditable, IOrientation, IRemoved, IIsVisible, IAddCommand, IRemoveCommand, IIsSelected, ISetIsSelected, 
        IIsRemovable, IIsReplicable, ISort, ISortOrder,
        IArrangement
    {
    }

    public interface IRemoveCommand
    {
        ICommand AddCommand { get; }
    }

    public interface IAddCommand
    {
        ICommand RemoveCommand { get; }

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
    public interface IArrangement
    {
        Arrangement Arrangement { get; set; }
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
