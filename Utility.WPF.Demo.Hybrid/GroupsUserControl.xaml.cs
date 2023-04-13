using System.Windows.Controls;

namespace Utility.WPF.Demo.Hybrid
{
    /// <summary>
    /// Interaction logic for GroupsUserControl.xaml
    /// </summary>
    public partial class GroupsUserControl : UserControl
    {
        public GroupsUserControl()
        {
            InitializeComponent();
        }

        private void MasterNotesControl_Change(object sender, Abstract.CollectionEventArgs e)
        {
        }
    }
}