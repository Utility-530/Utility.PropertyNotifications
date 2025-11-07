using System.Windows.Controls;
using Utility.WPF.Controls.Adorners;

namespace Utility.WPF.Demo.Lists
{
    /// <summary>
    /// Interaction logic for CheckBoxesUserControl.xaml
    /// </summary>
    public partial class CheckBoxesUserControl : UserControl
    {
        private bool flag;

        public CheckBoxesUserControl()
        {
            InitializeComponent();
            this.Loaded += CheckBoxesUserControl_Loaded;
        }

        private void CheckBoxesUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (flag)
                return;
            SettingsAdorner.AddTo(this.CheckBoxesListControl);
            flag = true;
        }
    }
}