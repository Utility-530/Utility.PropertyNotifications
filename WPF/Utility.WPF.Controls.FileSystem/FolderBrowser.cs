using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Helpers;
using Utility.WPF.Reactive;

namespace Utility.WPF.Controls.FileSystem
{
    public class FolderBrowser : FolderBrowser<TextBox>
    {
        public FolderBrowser()
        {
            textBoxContentChanges
            .OfType<TextBox>()
            .SelectMany(TextBoxHelper.Changes)
            .Subscribe(textChanges.OnNext);
        }

        public override void OnApplyTemplate()
        {
            var gridOne = GetTemplateChild("GridOne");
            var textBlock = (gridOne as FrameworkElement)?.Resources["TextBoxOne"] as TextBox ?? throw new NullReferenceException("GridOne is null");
            TextBoxContent = textBlock.Clone();
            base.OnApplyTemplate();
        }

        protected override void OnTextChange(string path, TextBox sender)
        {
            Helper.OnTextChange(path, sender);
            base.OnTextChange(path, sender);
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