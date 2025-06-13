using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility.WPF.Controls.Logs;
using Utility.Helpers.Ex;
using Utility.Entities;

namespace Utility.WPF.Demo.Logs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            LogsDataGrid.ItemsSource = ObservableCollectionHelper.ToObservableCollection(Globals.Logs);
            Globals.Logs.OnNext(new Log() { Message = "Main Window constructed", Date = DateTime.Now, Level = Enums.Diagnostic.Information });
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Test Exception");
            }
            catch (Exception ex)
            {
                Globals.Logs.OnNext(new Log() { Message= ex.Message, Date = DateTime.Now, Level = Enums.Diagnostic.Error});
            }
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Cast<object>().FirstOrDefault() is Log log)
            {
                try
                {
                    openStudioFileLine.LineNavigator.OpenFileAtLine(log.CallerFilePath, log.CallerLineNumber);
                }
                catch (Exception ex)
                {
                    Globals.Logs.OnNext(new Log() { Message = ex.Message, Date = DateTime.Now, Level = Enums.Diagnostic.Error });
                }
            }
        }
    }
}