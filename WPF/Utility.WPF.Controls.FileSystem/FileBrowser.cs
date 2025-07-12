using ReactiveUI;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.FileSystem
{

    public class FileComboBrowser : FileBrowser<ComboBoxTreeView>
    {
        FileSelectorBehavior behavior = new();
        protected readonly FileBrowserCommand fileBrowserCommand = new();
        public FileComboBrowser()
        {
            //contentChanges
            //.OfType<ComboBoxes.ComboBoxTreeView>()
            //.Subscribe(a =>
            //{   
            //});
        }

        private void A_SelectedNodeChanged(object sender, ComboBoxTreeView.SelectedNodeEventArgs e)
        {
            //pathChanges.OnNext();
            if(e.Value is IReadOnlyTree { Data: IFileSystemInfo { FileSystemInfo: { } info  } })
            {
                pathChanges.OnNext(info.FullName);
            }
        }

        public override void OnApplyTemplate()
        {
            //var gridOne = GetTemplateChild("GridOne");
            //var textBlock = (gridOne as FrameworkElement)?.Resources["TextBoxOne"] as TextBox ?? throw new NullReferenceException("GridOne is null");
            var treeView = new ComboBoxTreeView();
            treeView.MinWidth = 100;
            treeView.SelectedNodeChanged += A_SelectedNodeChanged;
         
     
            //behavior.SetBinding(XamComboEditorSelectedItemsBehavior.SelectedItemsProperty, new Binding()
            //{
            //    ElementName = "_uc",
            //    Path = new PropertyPath("SelectedItems"),
            //    Mode = BindingMode.TwoWay
            //});
            Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(treeView).Add(behavior);

            EditContent = treeView;
            base.OnApplyTemplate();
        }

        protected override void OnPathChange(string path, ComboBoxTreeView sender)
        {
            if(path!= behavior.FileSystemInfo?.FullName)
            {
                behavior.FileSystemInfo = System.IO.Path.GetExtension(path) is string ex ? new FileInfo(path) : new DirectoryInfo(path);
            }
            base.OnPathChange(path, sender);
        }

        protected override BrowserCommand Command => fileBrowserCommand;
    }

    public class FileBrowser : FileBrowser<TextBox>
    {
        public FileBrowser()
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