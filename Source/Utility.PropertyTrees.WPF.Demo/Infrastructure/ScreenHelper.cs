using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Utility.PropertyTrees.WPF.Demo.Infrastructure
{
    internal class ScreenHelper
    {
        public static void SetOnLastScreen(Window window)
        {
            Screen s = Screen.AllScreens[^1];
            System.Drawing.Rectangle r = s.WorkingArea;
            window.Top = r.Top;
            window.Left = r.Left;
            //window.WindowState = WindowState.Maximized;
        }     
        
        public static void SetOnFirstScreen(Window window)
        {
            Screen s = Screen.AllScreens[0];
            System.Drawing.Rectangle r = s.WorkingArea;
            window.Top = r.Top;
            window.Left = r.Left;
            //window.WindowState = WindowState.Maximized;
        }
    }
}
