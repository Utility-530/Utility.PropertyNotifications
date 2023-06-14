using CoffeeFlow.ViewModel;
using System.Windows;
using System.Windows.Input;
using Utility.Commands;

namespace CoffeeFlow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Command _toggleSidebar;
        public ICommand ToggleSidebarCommand
        {
            get { return _toggleSidebar ?? (_toggleSidebar = new Command(ToggleSidebar)); }
        }

        private Command _hideAvailableNodesCommand;

        public ICommand HideNodeListCommand
        {
            get { return _hideAvailableNodesCommand ?? (_hideAvailableNodesCommand = new Command(HideNodeList)); }
        }

        private Command _CloseAppCommand;

        public ICommand CloseAppCommand
        {
            get { return _CloseAppCommand ?? (_CloseAppCommand = new Command(Close)); }
        }

        private Command _showAvailableNodesCommand;

        public ICommand ShowNodeListCommand
        {
            get { return _showAvailableNodesCommand ?? (_showAvailableNodesCommand = new Command(ShowNodeList)); }
        }

        public bool IsNodePopupVisible = false;

        public void ToggleSidebar()
        {
            if (GridColumn1.Visibility == System.Windows.Visibility.Collapsed)
            {
                GridColumn1.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                GridColumn1.Visibility = System.Windows.Visibility.Collapsed;
            }

            HideNodeList();
        }

        public Point GetMouseLocation()
        {
            return Mouse.GetPosition(Application.Current.MainWindow);
        }

        public void ShowAtMousePosition(FrameworkElement UI)
        {
            Point p = GetMouseLocation();
            UI.Visibility = Visibility.Visible;

            UI.Margin = new Thickness(p.X, p.Y, 0, 0);

            IsNodePopupVisible = true;
        }

        public void ShowNodeList()
        {
            ShowAtMousePosition(lstAvailableNodes);
        }

        public void HideNodeList()
        {
            lstAvailableNodes.Visibility = Visibility.Collapsed;
            IsNodePopupVisible = false;
        }

        public void ShowStringList()
        {
            //ShowAtMousePosition(lstAvailableStrings);
        }

        public MainWindow()
        {
            InitializeComponent();

            //NetworkViewModel v = SimpleIoc.Default.GetInstance<NetworkViewModel>();
            //v.MainWindow = this;

            ToggleSidebar(); //turn sidebar off
            HideNodeList();
         
            /*
            InfoWindow info = new InfoWindow();
            info.ShowDialog();

            */
        }

      
    }
}
