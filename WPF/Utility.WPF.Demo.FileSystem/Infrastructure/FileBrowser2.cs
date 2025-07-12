using System.Windows.Controls;
using Utility.WPF.Controls.FileSystem;

namespace Utility.WPF.Demo.FileSystem.Infrastructure
{
    public class FileBrowser2 : FileBrowser<TextBlock>
    {
        private TextBlock TextBlockOne;

        public FileBrowser2()
        {
            TextBlockOne = new TextBlock { Width = 300, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            this.EditContent = TextBlockOne;
        }

        protected override void OnPathChange(string path, TextBlock textBox)
        {
            TextBlockOne.Text = path;
            TextBlockOne.Focus();
            var length = System.IO.Path.GetFileName(path).Length;
            TextBlockOne.ToolTip = path;
        }
    }
}