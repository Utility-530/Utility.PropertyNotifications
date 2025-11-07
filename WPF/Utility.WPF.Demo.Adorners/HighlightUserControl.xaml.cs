using System.Windows.Controls;
using System.Windows.Input;
using WpfHighlighter;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for HighlightUserControl.xaml
    /// </summary>
    public partial class HighlightUserControl : UserControl
    {
        public HighlightUserControl()
        {
            InitializeComponent();
            this.MouseDoubleClick += HighlightUserControl_MouseDoubleClick;
        }

        private void HighlightUserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HighlightCommand.Instance.Execute(this);
        }
    }
}