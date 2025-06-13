using Utility.Trees.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Enums;
using System.Collections;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Utility.Structs;
using System;

namespace Utility.Interfaces.Exs
{
    public interface INode : ITree, IIsExpanded, ICurrent<INode>, IAdd, IKey, ILocalIndex, IIsPersistable, IIsEditable, IOrientation, IRemoved, IIsVisible, IAddCommand, IRemoveCommand, IIsSelected, ISetIsSelected,
        IIsRemovable, IIsReplicable, ISort, ISortOrder,
        IArrangement, IRows, IColumns, IIsAugmentable, IErrors, IConnectorPosition, IDataTemplate
    {

    }

    public interface IErrors
    {
        ObservableCollection<Exception> Errors { get; }
    }

    public interface IColumns
    {
        ObservableCollection<Dimension> Columns { get; }
    }
     
    public interface IRows
    {
        ObservableCollection<Dimension> Rows { get; }
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
    public interface IGetNode
    {
        public INode Node { get; }
    }

    public interface IIsAugmentable
    {
        bool IsAugmentable { get; set; }
    }

    public interface IConnectorPosition
    {
        Position2D ConnectorPosition { get; set; }
    }
    public interface IIsReactivationRequired
    {
        bool IsReactivationRequested { get; set; }
    }
    public interface IDataTemplate
    {
        string DataTemplate { get; set; }
    }
}
