using System.Windows;
using System.Windows.Controls;
using Tiny.Toolkits;
using Utility.Helpers.NonGeneric;

namespace Utility.Trees.Demo.MVVM.Views
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
