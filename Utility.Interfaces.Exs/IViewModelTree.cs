using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Structs;
using Utility.Trees.Abstractions;

namespace Utility.Interfaces.Exs
{
    public interface IViewModelTree :
        ITree, 
        IIsExpanded, 
        ICurrent<IViewModelTree>, 
        IAdd, 
        ILocalIndex, 
        IIsEditable, 
        IOrientation,
        IRemoved, 
        IIsVisible, 
        IIsSelected, 
        IIsRemovable,
        IIsReplicable,
        ISort, 
        ISortOrder,      
        IIsChildrenRefreshed,
        IIsActive,
        IArrangement,
        IRows,
        IColumns, 
        IIsAugmentable,
        IErrors, 
        IConnectorPosition,
        IDataTemplate,
        ISelectedItemTemplate,
        IItemsPanelTemplate, 
        ITitle,
        IIsReadOnly,
        ILocation,
        IGetIsSingular,
        //IIsChildrenTracked,
        IShouldValueBeTracked,
        IDoesValueRequireLoading,
        IDoesValueRequireSaving,
        IIsValueLoaded,
        IAreChildrenLoaded,
        IAreChildrenTracked,
        IIsProliferable,
        IIsLoaded,
        IStyle,
        ILayout,
        IIsChecked

    //IHeight,
    //IWidth
    {
    }

    public interface IStyle
    {
        Visual Style { get; set; }
    }
    public interface ILayout
    {
        VisualLayout Layout { get; set; }
    }
    public interface IIsLoaded
    {
        bool IsLoaded { get; set; }
    }

    public interface IIsProliferable
    {
        bool IsProliferable { get; set; }
    }

    public interface ISelectedItemTemplate
    {
        string SelectedItemTemplate { get; set; }
    }

    public interface IDoesValueRequireLoading
    {
        bool DoesValueRequireLoading { get; set; }
    }
    public interface IAreChildrenLoaded
    {
        bool AreChildrenLoaded { get; set; }
    }
    public interface IAreChildrenTracked
    {
        bool AreChildrenTracked { get; set; }
    }
    public interface IIsValueLoaded
    {
        bool IsValueLoaded { get; set; }
    }

    public interface IDoesValueRequireSaving
    {
        bool DoesValueRequireSaving { get; set; }
    }

    public interface IHeight
    {
        double? Height { get; set; }
    }

    public interface IWidth
    {
        double? Width { get; set; }
    }

    public interface IShouldValueBeTracked
    {
        bool ShouldValueBeTracked { get; set; }
    }

    //public interface IIsChildrenTracked
    //{
    //    bool IsChildrenTracked { get; set; }
    //}

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

    public interface IIsChecked
    {
        bool IsChecked { get; set; }
    }
}