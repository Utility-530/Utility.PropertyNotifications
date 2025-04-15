using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Commands;
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
                if (o is EditRoutedEventArgs args)
                {
                    Items.Add(args.Edit as Character);
                }
            });

            Items = new(Data.Resources.Instance["Characters"] as Character[]);

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
