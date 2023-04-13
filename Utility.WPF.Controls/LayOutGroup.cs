using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    //https://www.markwithall.com/programming/2014/05/02/labelled-textbox-in-wpf.html
    public class LayoutGroup : StackPanel
    {
        public LayoutGroup()
        {
            Grid.SetIsSharedSizeScope(this, true);
        }
    }
}