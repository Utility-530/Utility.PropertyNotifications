using System.Windows;

namespace Utility.WPF.Controls.Date
{

    public class MonthListControl : MonthControl
    {
        static MonthListControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthListControl), new FrameworkPropertyMetadata(typeof(MonthListControl)));
        }
    }
}