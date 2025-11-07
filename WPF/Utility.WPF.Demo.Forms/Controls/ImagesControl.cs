using Utility.Enums;
using Utility.WPF.Controls.FileSystem;
using Utility.WPF.Controls.Hybrid;

namespace Utility.WPF.Demo.Forms.Controls
{
    internal class ImagesControl : MasterListControl
    {
        private readonly FileBrowserCommand command;

        public ImagesControl()
        {
            command = new FileBrowserCommand();
            var (fileter, ext) = FileBrowserCommand.GetImageFilterAndExtension();
            command.IsMultiSelect = true;
            command.Filter = fileter;
            command.Extension = ext;
            command.TextChanged += Command_TextChanged;
            //  RaiseEvent(new CollectionEventArgs(EventType.Add, null, -1, ChangeEvent));
        }

        private void Command_TextChanged(string obj)
        {
            RaiseEvent(new Utility.WPF.Abstract.CollectionItemEventArgs(EventType.Add, obj, -1, ChangeEvent));
        }

        protected override void ExecuteAdd()
        {
            command.Execute(null);
        }
    }
}