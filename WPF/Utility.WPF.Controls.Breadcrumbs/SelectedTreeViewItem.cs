using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Breadcrumbs
{
    public class SelectedTreeViewItem : TreeViewItem
    {
        static SelectedTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectedTreeViewItem), new FrameworkPropertyMetadata(typeof(SelectedTreeViewItem)));
        }
    } 
}
