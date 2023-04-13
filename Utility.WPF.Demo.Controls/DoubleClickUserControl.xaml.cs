using ReactiveUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Demo.Controls
{
    /// <summary>
    /// Interaction logic for DoubleClickUserControl.xaml
    /// </summary>
    public partial class DoubleClickUserControl : UserControl
    {
        public DoubleClickUserControl()
        {
            InitializeComponent();
        }
    }

    public class DoubleClickViewModel
    {
        public ICommand MyCommand { get; set; }

        public DoubleClickViewModel()
        {
            MyCommand = ReactiveCommand.Create(() => MessageBox.Show("double click!"));
        }
    }
}