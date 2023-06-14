using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Clocks
{
    public class SwitchClock : Control
    {
        static SwitchClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchClock), new FrameworkPropertyMetadata(typeof(SwitchClock)));
        }
    }
}