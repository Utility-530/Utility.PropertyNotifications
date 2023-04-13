using MaterialDesignThemes.Wpf;

namespace Utility.WPF.Controls.FileSystem
{
    public class DirectoryControl : PathControl
    {
        static DirectoryControl()
        {
            //  DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryControl), new FrameworkPropertyMetadata(typeof(DirectoryControl)));
        }

        public DirectoryControl()
        {
            Icon = PackIconKind.Folder;
        }
    }
}