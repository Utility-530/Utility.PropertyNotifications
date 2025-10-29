using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Structs;
using Utility.Trees.Abstractions;

namespace Utility.Interfaces.Exs
{
    public interface IViewModelTree : ITree, IIsExpanded, ICurrent<IViewModelTree>, IAdd, IKey, ILocalIndex, IIsEditable, IOrientation,
        IRemoved, IIsVisible, IIsSelected, ISetIsSelected,
        IIsRemovable, IIsReplicable, ISort, ISortOrder,
        IName,
        IIsChildrenRefreshed,
        IIsActive,
        IArrangement, IRows, IColumns, IIsAugmentable, IErrors, IConnectorPosition, IDataTemplate, IItemsPanelTemplate, ITitle, IIsContentVisible,
        IGetGuid,
        IIsReadOnly,
        ILocation,
        IGetIsSingular,
        IIsChildrenTracked,
        IIsValueTracked
    //IIsPersistable
    //IHeight,
    //IWidth
    {
    }

    public interface IHeight
    {
        double? Height { get; set; }
    }

    public interface IWidth
    {
        double? Width { get; set; }
    }

    public interface IIsValueTracked
    {
        bool IsValueTracked { get; set; }
    }

    public interface IIsChildrenTracked
    {
        bool IsChildrenTracked { get; set; }
    }

    public interface IIsChildrenRefreshed
    {
        bool IsChildrenRefreshed { get; set; }
    }

    public interface IIsContentVisible
    {
        bool? IsContentVisible { get; set; }
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

    public interface IItemsPanelTemplate
    {
        string ItemsPanelTemplate { get; set; }
    }
}