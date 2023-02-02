using Utility.Enums;
using UtilityWpf.Controls.FileSystem;
using UtilityWpf.Controls.Hybrid;

namespace UtilityWpf.Demo.Forms.Controls
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
            RaiseEvent(new UtilityWpf.Abstract.CollectionItemEventArgs(EventType.Add, obj, -1, ChangeEvent));
        }

        protected override void ExecuteAdd()
        {
            command.Execute(null);
        }
    }

}