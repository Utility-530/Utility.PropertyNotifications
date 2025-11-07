using System.Windows;

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