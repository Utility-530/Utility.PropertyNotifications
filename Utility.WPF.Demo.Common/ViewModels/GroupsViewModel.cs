using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class GroupsViewModel
    {
        public GroupsViewModel()
        {
            Data = new ObservableCollection<object> { new RowBaseViewModel(), new RowBaseViewModel(), new NotesViewModel(), };
            //ChangeCommand = ReactiveUI.ReactiveCommand.Create<object, Unit>((a) =>
            //{
            //    switch (a)
            //    {
            //        case MovementEventArgs eventArgs:
            //            foreach (var item in eventArgs.Changes)
            //            {
            //                Data.Move(item.OldIndex, item.Index);
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //    return Unit.Default;
            //});
        }

        public ObservableCollection<object> Data { get; }

        public ICommand ChangeCommand { get; }
    }
}