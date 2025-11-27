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
    public record TabServiceAddEngineParam() : Param<TabService>(nameof(TabService.Add), "root");
    public record TabServiceRemoveEngineParam() : Param<TabService>(nameof(TabService.Remove), "root");
    public record TabServiceAddItemParam() : Param<TabService>(nameof(TabService.Add), "addItem");
    public record TabServiceRemoveItemParam() : Param<TabService>(nameof(TabService.Remove), "removeItem");

    public class TabService
    {
        public static void Add(INodeRoot root, AddItemActionCallbackArgs addItem)
        {
            if (addItem.NewItem is not IGetName getName)
                throw new Exception("sd32 11111");

            root
                .Create(addItem.NewItem)
                .Subscribe(x =>
                {
                    (addItem.Owner.DataContext as ITree).Add(addItem.NewItem);
                });
        }

        public static void Remove(INodeRoot root, ItemActionCallbackArgs<TreeViewItem> removeItem)
        {
            if (removeItem.DragablzItem.DataContext is INodeViewModel viewModel)
                root.Destroy(viewModel.Key());
            else
                throw new Exception("$ bdf33");
        }
    }
}
