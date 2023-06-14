using DateWork.Models;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Controls.Date.Model;

namespace Utility.WPF.Demo.Date
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
       readonly DateModel dateModel = new DateMonthModel();

        public Window1()
        {
            InitializeComponent();
            DatePickerControl.Month = dateModel.Month;
            DatePickerControl.Year = dateModel.Year;
            MonthGridControl.ItemsSource = dateModel.Days;
        }

        private void DatePickerControl_DateChange(object sender, Utility.WPF.Controls.Date.DateChangeEventArgs e)
        {
            dateModel.Month = e.Month;
            dateModel.Year = e.Year;
            MonthGridControl.ItemsSource = dateModel.Days;
            MonthGridControl.SelectedItem = dateModel.Current;
        }

        private void MonthGridControl_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = MonthGridControl.SelectedItem;
        }
    }
}
