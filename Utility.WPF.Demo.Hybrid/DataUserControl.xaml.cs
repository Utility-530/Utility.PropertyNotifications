using System.Windows.Controls;

namespace Utility.WPF.Demo.Hybrid
{
    /// <summary>
    /// Interaction logic for DataUserControl.xaml
    /// </summary>
    public partial class DataUserControl : UserControl
    {
        public DataUserControl()
        {
            InitializeComponent();
        }

        private void MasterNotesControl_Change(object sender, Abstract.CollectionEventArgs e)
        {
        }
    }
}