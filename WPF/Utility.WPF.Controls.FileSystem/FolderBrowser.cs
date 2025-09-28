using System;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.FileSystem
{
    public class FolderComboBrowser : FolderBrowser<ComboBoxTreeView>
    {
        FileSelectorBehavior behavior = new() { ExcludesFiles = true };

        public FolderComboBrowser()
        {
        }

        private void A_SelectedNodeChanged(object sender, ComboBoxTreeView.SelectedNodeEventArgs e)
        {
            //pathChanges.OnNext();
            if (e.Value is IGetData { Data: IFileSystemInfo { FileSystemInfo: { } info } })
            {
                pathChanges.OnNext(info.FullName);
            }
        }

        public override void OnApplyTemplate()
        {
            ComboBoxTreeView treeView = new()
            {
                MinWidth = 100
            };
            treeView.SelectedNodeChanged += A_SelectedNodeChanged;

            Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(treeView).Add(behavior);

            EditContent = treeView;
            base.OnApplyTemplate();
        }

        protected override void OnPathChange(string path, ComboBoxTreeView sender)
        {
            if (path != behavior.FileSystemInfo?.FullName)
            {
                behavior.FileSystemInfo = System.IO.Path.GetExtension(path) is string ex ? new FileInfo(path) : new DirectoryInfo(path);
            }
            base.OnPathChange(path, sender);
        }
    }

    public class FolderBrowser : FolderBrowser<TextBox>
    {
        public FolderBrowser()
        {
            contentChanges
            .OfType<TextBox>()
            .SelectMany(TextBoxHelper.Changes)
            .Subscribe(pathChanges.OnNext);
        }

        public override void OnApplyTemplate()
        {
            var gridOne = GetTemplateChild("GridOne");
            var textBlock = (gridOne as FrameworkElement)?.Resources["TextBoxOne"] as TextBox ?? throw new NullReferenceException("GridOne is null");
            EditContent = textBlock.Clone();
            base.OnApplyTemplate();
        }

        protected override void OnPathChange(string path, TextBox sender)
        {
            Helper.OnTextChange(path, sender);
            base.OnPathChange(path, sender);
        }
    }

    public class FolderBrowser<T> : PathBrowser<T> where T : Control
    {
        public FolderBrowser()
        {
        }

        protected override BrowserCommand Command { get; } = new FolderBrowserCommand();
    }
}