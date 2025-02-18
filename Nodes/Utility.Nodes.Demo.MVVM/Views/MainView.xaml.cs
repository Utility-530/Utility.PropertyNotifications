using System.Windows.Controls;

namespace Utility.Nodes.Demo.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        private object content;

        private MainView()
        {
            InitializeComponent();


        }
  
        public static MainView Instance { get; } = new();


    }
}
