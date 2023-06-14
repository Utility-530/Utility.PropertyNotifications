using ReactiveUI;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class NoteViewModel : ReactiveObject
    {
        private string text;

        public NoteViewModel(string text)
        {
            Text = text;
        }

        public string Text { get => text; set => this.RaiseAndSetIfChanged(ref text, value); }
    }
}