using Utility.PropertyNotifications;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class NoteViewModel : NotifyPropertyClass
    {
        private string text;

        public NoteViewModel(string text)
        {
            Text = text;
        }

        public string Text { get => text; set => this.RaisePropertyChanged(ref text, value); }
    }
}