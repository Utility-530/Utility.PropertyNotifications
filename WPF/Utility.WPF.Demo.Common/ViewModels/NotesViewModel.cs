using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Endless;
using Microsoft.Xaml.Behaviors.Core;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class NotesViewModel
    {
        private ICommand changeCommand;

        private bool isReadOnly;

        public bool IsReadOnly
        {
            get => isReadOnly; set => isReadOnly = value;
        }

        public string Header { get; } = "NotesViewModel";

        public virtual ObservableCollection<NoteViewModel> Collection { get; } = new ObservableCollection<NoteViewModel> { };

        public System.Collections.IEnumerator NewItem { get => 0.Repeat().Select(a => new NoteViewModel(a.ToString())).GetEnumerator(); }

        public ICommand ChangeCommand => changeCommand ??= new ActionCommand(Change);

        private void Change()
        {
        }
    }
}