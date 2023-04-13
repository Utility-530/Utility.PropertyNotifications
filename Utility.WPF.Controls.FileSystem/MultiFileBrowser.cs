using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Utility.WPF.Controls.FileSystem
{
    public class MultiFileBrowser : FileBrowser<ListBox>
    {
        public MultiFileBrowser()
        {
            this.TextBoxContent = new ListBox();
            fileBrowserCommand.IsMultiSelect = true;
        }

        protected override void OnTextChange(string path, ListBox sender)
        {
            ((ObservableCollection<string>)(sender.ItemsSource ??= new ObservableCollection<string>())).Add(path);
            base.OnTextChange(path, sender);
        }
    }
}