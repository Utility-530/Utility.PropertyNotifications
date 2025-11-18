using System.Windows.Controls;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Services.Meta;
using Utility.Trees.Abstractions;
using static Utility.WPF.Controls.Trees.Infrastructure.TreeTabHelper;
using Utility.Interfaces;

namespace Utility.Nodes.Demo.Editor.Services
{
    public record TabServiceAddEngineParam() : Param<TabService>(nameof(TabService.Add), "engine");
    public record TabServiceRemoveEngineParam() : Param<TabService>(nameof(TabService.Remove), "engine");
    public record TabServiceAddItemParam() : Param<TabService>(nameof(TabService.Add), "addItem");
    public record TabServiceRemoveItemParam() : Param<TabService>(nameof(TabService.Remove), "removeItem");

    public class TabService
    {
        public static void Add(INodeSource engine, AddItemActionCallbackArgs addItem)
        {
            if (addItem.NewItem is not IGetName getName)
                throw new Exception("sd32 11111");

            engine
                .Create(getName.Name, Guid.NewGuid(), name =>
                {
                    return addItem.NewItem;
                })
                .Subscribe(x =>
                {
                    (addItem.Owner.DataContext as ITree).Add(addItem.NewItem);
                });
        }

        public static void Remove(INodeSource engine, ItemActionCallbackArgs<TreeViewItem> removeItem)
        {
            if (removeItem.DragablzItem.DataContext is INodeViewModel viewModel)
                engine.Remove(a => a.Guid == viewModel.Guid);
            else
                throw new Exception("$ bdf33");
        }
    }
}
