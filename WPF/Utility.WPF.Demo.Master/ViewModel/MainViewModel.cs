using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using Utility.Commands;
using Utility.EventArguments;

namespace Utility.WPF.Demo.Master.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            Rows = new ObservableCollection<RowViewModel> { new RowViewModel(), new RowViewModel(), };
            ChangeCommand = new Command<object>((a) =>
            {
                switch (a)
                {
                    case MovementEventArgs eventArgs:
                        foreach (var item in eventArgs.Changes)
                        {
                            Rows.Move(item.OldIndex, item.Index);
                        }
                        break;

                    default:
                        break;
                }
            });
        }

        public ObservableCollection<RowViewModel> Rows { get; }

        public ICommand ChangeCommand { get; }
    }
}