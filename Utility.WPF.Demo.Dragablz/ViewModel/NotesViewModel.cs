using System.Collections.ObjectModel;
using Utility.WPF.Demo.Common.ViewModels;

namespace Utility.WPF.Demo.Dragablz.ViewModels
{
    internal class NotesViewModel : Utility.WPF.Demo.Common.ViewModels.NotesViewModel
    {
        public override ObservableCollection<NoteViewModel> Collection { get; } = new ObservableCollection<NoteViewModel> {
        new NoteViewModel("sdsfd"),
        new NoteViewModel("sds333d"),
        };
    }
}