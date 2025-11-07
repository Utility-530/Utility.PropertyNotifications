using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Commands;
using Utility.Persists;
using Utility.WPF.Demo.Data.Model;

namespace Utility.WPF.Demo.Lists
{
    /// <summary>
    /// Interaction logic for CustomSelectorUserControl.xaml
    /// </summary>
    public partial class CustomSelectorUserControl : UserControl
    {
        public CustomSelectorUserControl()
        {
            InitializeComponent();
        }
    }

    public class CustomSelectorViewModel
    {
        private Character newObject = new Character() { Color = Colors.Aqua };

        public CustomSelectorViewModel()
        {
            FinishEdit = new Command<object>(o =>
            {
                if (o is EditRoutedEventArgs { IsAccepted: true } args)
                {
                    Items.Add(args.Edit as Character);
                }
                else
                {
                    MessageBox.Show("Rejected");
                }
            });

            Items = new(Data.Resources.Instance["Characters"] as Character[]);
            Items.ToManager(a => (a as Character).Id);
        }

        public ICommand FinishEdit { get; }

        public ObservableCollection<Character> Items { get; }

        public Character Edit
        {
            get => newObject; set
            {
                newObject = value;
            }
        }
    }
}