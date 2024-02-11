using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.FileSystem
{
    public class FileBrowser : FileBrowser<TextBox>
    {
        public FileBrowser()
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

    public class FileBrowser<T> : PathBrowser<T> where T : FrameworkElement
    {
        private readonly Subject<string> filterChanges = new Subject<string>();
        private readonly Subject<string> extensionChanges = new Subject<string>();

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FileBrowser<T>), new PropertyMetadata(null, FilterChanged));

        public static readonly DependencyProperty ExtensionProperty =
            DependencyProperty.Register("Extension", typeof(string), typeof(FileBrowser<T>), new PropertyMetadata(null, ExtensionChanged));

        protected readonly FileBrowserCommand fileBrowserCommand = new();

        public FileBrowser()
        {
            this.WhenAnyValue(a => a.Filter)
                .Subscribe(a => { fileBrowserCommand.Filter = a; });
            this.WhenAnyValue(a => a.Extension)
                .Subscribe(a => { fileBrowserCommand.Extension = a; });
        }

        public string Filter
        {
            get => (string)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public string Extension
        {
            get => (string)GetValue(ExtensionProperty);
            set => SetValue(ExtensionProperty, value);
        }

        protected override BrowserCommand Command => fileBrowserCommand;

        private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FileBrowser<T>)?.filterChanges.OnNext((string)e.NewValue);
        }

        private static void ExtensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FileBrowser<T> ?? throw new NullReferenceException("FileBrowser object is null")).extensionChanges.OnNext((string)e.NewValue);
        }
    }
}