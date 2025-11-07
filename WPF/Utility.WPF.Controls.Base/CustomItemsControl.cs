using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Base
{
    /// <summary>
    /// Makes it possible to configure ItemContainerStyle
    /// </summary>
    public class CustomItemsControl : ItemsControl
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContentControl();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            // Even wrap other ContentControls
            return false;
        }
    }
}