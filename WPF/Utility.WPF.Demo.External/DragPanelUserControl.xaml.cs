using System.Windows.Controls;
using Utility.Commands;

namespace Utility.WPF.Demo.External
{
    /// <summary>
    /// Interaction logic for ContainerUserControl.xaml
    /// </summary>
    public partial class DragPanelUserControl : UserControl
    {
        public DragPanelUserControl()
        {
            this.InitializeComponent();
        }

        private Command<int[]> _swap;

        public Command<int[]> SwapCommand
        {
            get
            {
                if (_swap == null)
                    _swap = new Command<int[]>(
                        (indexes) =>
                        {
                            int fromS = indexes[0];
                            int to = indexes[1];
                            var elementSource = listBox.Items[to];
                            var dragged = listBox.Items[fromS];
                            if (fromS > to)
                            {
                                listBox.Items.Remove(dragged);
                                listBox.Items.Insert(to, dragged);
                            }
                            else
                            {
                                listBox.Items.Remove(dragged);
                                listBox.Items.Insert(to, dragged);
                            }
                        }
                    );
                return _swap;
            }
        }
    }
}