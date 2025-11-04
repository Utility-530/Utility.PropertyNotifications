using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfScreenHelper;

namespace Utility.WPF.Helpers
{
    public static class WindowHelpers
    {
        public static void ToLeft(this Window window)
        {
            // Find the leftmost screen
            Screen leftmostScreen = Screen.AllScreens
                .OrderBy(s => s.Bounds.Left)
                .First();

            // Get its working area (excluding taskbar)
            var area = leftmostScreen.WorkingArea;
            window.Left = area.Left;
            window.Top = area.Top;
            window.Width = area.Width;
            window.Height = area.Height;
        }
        public static void ToRight(this Window window)
        {
            // Find the leftmost screen
            Screen rightMostScreen = Screen.AllScreens
                .OrderBy(s => s.Bounds.Right)
                .First();

            // Get its working area (excluding taskbar)
            var area = rightMostScreen.WorkingArea;
            window.Left = area.Left;
            window.Top = area.Top;
            window.Width = area.Width;
            window.Height = area.Height;
        }

    }
}
