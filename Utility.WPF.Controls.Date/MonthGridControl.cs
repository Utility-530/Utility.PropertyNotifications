using ReactiveUI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Utility.WPF;

namespace Utility.WPF.Controls.Date
{
    public class MonthGridControl : MonthControl
    {

        static MonthGridControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthGridControl), new FrameworkPropertyMetadata(typeof(MonthGridControl)));
        }
    }
}