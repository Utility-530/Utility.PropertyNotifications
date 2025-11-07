using System.Linq;
using System.Windows;
using WpfScreenHelper;

namespace Utility.WPF.Packages
{
    public static class ScreenHelper
    {
        public static void SetOnLastScreen(this Window window)
        {
            Screen s = Screen.AllScreens.ToArray()[^1];
            var r = s.Bounds;
            window.Top = r.Top;
            window.Left = r.Left;
            //window.WindowState = WindowState.Maximized;
        }

        public static void SetOnFirstScreen(this Window window)
        {
            Screen s = Screen.AllScreens.ToArray()[0];
            var r = s.Bounds;
            window.Top = r.Top;
            window.Left = r.Left;
            //window.WindowState = WindowState.Maximized;
        }
    }
}