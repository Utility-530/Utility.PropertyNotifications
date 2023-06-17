using System.Windows;
using Utility.WPF.Controls.Trees;

namespace Utility.PropertyTrees.WPF
{
    /// <summary>
    /// Interaction logic for PropertyTreeList.xaml
    /// </summary>
    public partial class PropertyTreeList : TreeListView
    {
        public PropertyTreeList()
        {
            InitializeComponent();
            this.SizeChanged += Tree_SizeChanged; 
        }

        private void Tree_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TreeListView _ListView = sender as TreeListView;
            var _ActualWidth = _ListView.ActualWidth - SystemParameters.VerticalScrollBarWidth - _ListView.Columns[0].Width;
            var separateWidth = (_ActualWidth * 1d) / (_ListView.Columns.Count - 1);
            for (int i = 1; i < _ListView.Columns.Count; i++)
            {
                _ListView.Columns[i].Width = separateWidth;
            }
        }
    }
}
