using System.Windows;
using System.Windows.Controls;
using Utility.Commands;
using Utility.WPF.Controls.Buttons;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.Buttons
{
    /// <summary>
    /// Interaction logic for CustomButtonsUserControl.xaml
    /// </summary>
    public partial class EnumButtonsUserControl : UserControl
    {
        public EnumButtonsUserControl()
        {
            InitializeComponent();

            this.Loaded += EnumButtonsUserControl_Loaded;
        }

        private void EnumButtonsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var command = new Command<object>((a) => OutputListBox.Items.Add(new ListBoxItem { Content = a.ToString() }));
            foreach (var child in this.ChildrenOfType<EnumButtons>())
                child.Command = command;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditCollectionButtons.Visible = (System.Enum)Enums.AddRemove.Remove;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EditCollectionButtons.Enabled = Enums.AddRemove.Add;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            {
                PropertyMetadata pmd = Controls.Buttons.EditCollectionButtons.VisibleProperty.GetMetadata(EditCollectionButtons);

                EditCollectionButtons.Visible = (Enums.AddRemove?)pmd.DefaultValue;
            }
            {
                PropertyMetadata pmd = Controls.Buttons.EditCollectionButtons.EnabledProperty.GetMetadata(EditCollectionButtons);

                EditCollectionButtons.Enabled = (Enums.AddRemove?)pmd.DefaultValue;
            }
        }

        private void Forward_Hide_Click(object sender, RoutedEventArgs e)
        {
            NavigationButtons.Visible = Enums.Direction.Up | Enums.Direction.Left | Enums.Direction.Down;
        }

        private void Up_Disable_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationButtons.Enabled = Enums.Direction.Right | Enums.Direction.Left | Enums.Direction.Down;
        }

        private void Reset_Click_1(object sender, RoutedEventArgs e)
        {
            {
                PropertyMetadata pmd = Controls.Buttons.EnumButtons.VisibleProperty.GetMetadata(NavigationButtons);

                NavigationButtons.Visible = (Enums.Direction)pmd.DefaultValue;
            }
            {
                PropertyMetadata pmd = Controls.Buttons.EnumButtons.EnabledProperty.GetMetadata(NavigationButtons);

                NavigationButtons.Enabled = (Enums.Direction)pmd.DefaultValue;
            }
        }
    }
}