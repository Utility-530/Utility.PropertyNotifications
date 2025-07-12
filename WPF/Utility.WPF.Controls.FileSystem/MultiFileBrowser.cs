using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Utility.WPF.Controls.FileSystem
{
    public class MultiFileBrowser : FileBrowser<ListBox>
    {
        public MultiFileBrowser()
        {
            this.EditContent = new ListBox();
            fileBrowserCommand.IsMultiSelect = true;
        }

        protected override void OnPathChange(string path, ListBox sender)
        {
            ((ObservableCollection<string>)(sender.ItemsSource ??= new ObservableCollection<string>())).Add(path);
            base.OnPathChange(path, sender);
        }
    }
}