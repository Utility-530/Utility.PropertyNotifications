using System;
using System.Windows.Controls;
using Utility.Commands;
using Utility.Interfaces.Generic;
using Utility.Trees.Abstractions;
using Utility.Trees.WPF.Abstractions;
using Utility.WPF.Controls.Trees;

namespace Utility.Trees.Demo.MVVM.MVVM
{
    public partial class Data
    {
        public class TreeViewItemFactory : ITreeViewItemFactory
        {
            Random random = new();
            public HeaderedItemsControl Make(object instance)
            {

                var item = new CustomTreeViewItem
                {
                    //AddCommand = new Command(() => { if (instance is ITree { } item) item.Add(new ModelTree(Helpers.Names.Random(random), Guid.NewGuid(), ((GuidKey)item.Key).Value)); }),
                    RemoveCommand = new Command(() => { if (instance is IParent<ITree> { Parent: { } parent }) parent.Remove(instance); }),
                    Header = instance,
                    DataContext = instance
                };
                return item;
            }

            public static TreeViewItemFactory Instance { get; } = new();
        }


    }
}
