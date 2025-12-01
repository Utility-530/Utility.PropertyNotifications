using System.Collections;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Ex;
using Utility.Reactives;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;
using Utility.WPF.Factorys;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class FileSelectorBehavior : TreeSelectorBehavior
    {
        public static readonly DependencyProperty DirectoriesProperty = DependencyProperty.Register("Assemblies", typeof(IEnumerable), typeof(FileSelectorBehavior), new PropertyMetadata(AssembliesChanged));
        public static readonly DependencyProperty FileSystemInfoProperty = DependencyProperty.Register("FileSystemInfo", typeof(FileSystemInfo), typeof(FileSelectorBehavior), new PropertyMetadata(FileSystemInfoChanged));
        public static readonly DependencyProperty UseEntryAssemblyProperty = DependencyProperty.Register("UseEntryAssembly", typeof(bool), typeof(FileSelectorBehavior), new PropertyMetadata(true));
        public static readonly DependencyProperty ExcludesFilesProperty = DependencyProperty.Register("ExcludesFiles", typeof(bool), typeof(FileSelectorBehavior), new PropertyMetadata(false));

        private static void FileSystemInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileSelectorBehavior { AssociatedObject.ItemsSource: IReadOnlyTree tree } FileSystemInfoSelector)
            {
                if (e.NewValue is FileSystemInfo FileSystemInfo)
                    FileSystemInfoSelector.ChangeFileSystemInfo(tree, FileSystemInfo);
            }
        }

        private static void AssembliesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileSelectorBehavior FileSystemInfoSelector && e.NewValue is IEnumerable enumerable)
            {
                set(FileSystemInfoSelector, enumerable);
                if (enumerable is IReadOnlyTree tree && FileSystemInfoSelector.FileSystemInfo is FileSystemInfo FileSystemInfo)
                    FileSystemInfoSelector.ChangeFileSystemInfo(tree, FileSystemInfo);
            }
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.SelectedItemTemplateSelector = CustomItemTemplateSelector.Instance;

            this.AssociatedObject.Observe(a => a.SelectedNode)
                .Subscribe(a =>
                {
                    if (a is DirectoryModel tree)
                    {
                        var x = tree.Items().OfType<Model<string>>().ToArray().ToNodeViewModel(t_tree: tree);

                        if (tree.FileSystemInfo is FileSystemInfo fileSystemInfo)
                            FileSystemInfo = fileSystemInfo;
                        else if (tree is IFileSystemInfo { FileSystemInfo: { } _FileSystemInfo })
                        {
                            FileSystemInfo = _FileSystemInfo;
                        }
                    }
                });

            if (Directories == null || UseEntryAssembly)
            {
                Directories = new List<DirectoryInfo>([new DirectoryInfo("c:\\"), new DirectoryInfo("o:\\")]);
            }
            if (Directories != null)
                set(this, Directories);

            AssociatedObject.OnLoaded(a =>
            {
                if (FileSystemInfo is FileSystemInfo fileSystemInfo && AssociatedObject.TreeView != null && this.AssociatedObject.ItemsSource is IReadOnlyTree tree)
                {
                    ChangeFileSystemInfo(tree, fileSystemInfo);
                }
            });

            base.OnAttached();
        }

        private static void set(FileSelectorBehavior FileSystemInfoSelector, IEnumerable enumerable)
        {
            var assemblyTree = enumerable.Cast<DirectoryInfo>().Select(a => DirectoryModel.Create(a, FileSystemInfoSelector.ExcludesFiles == false)).ToArray().ToNodeViewModel();
            FileSystemInfoSelector.AssociatedObject.ItemsSource = assemblyTree;
        }

        private void ChangeFileSystemInfo(IReadOnlyTree tree, FileSystemInfo _FileSystemInfo)
        {
            if (tree.Descendant(a => filter(_FileSystemInfo, a)) is IReadOnlyTree { } innerTree)
            {
                AssociatedObject.IsError = false;
                AssociatedObject.UpdateSelectedItems(innerTree);
                if (AssociatedObject.TreeView?.ItemContainerGenerator.ContainerFromItem(AssociatedObject.TreeView.SelectedItem) is TreeViewItem item)
                    item.IsSelected = true;
                AssociatedObject.SelectedNode = innerTree;
            }
            else
            {
                AssociatedObject.IsError = true;
            }
        }

        private static bool filter(FileSystemInfo _FileSystemInfo, (IGetValue tree, int level) a)
        {
            switch (a.tree.Value)
            {
                case "root":
                    return false;

                case IFileSystemInfo { FileSystemInfo.FullName: { } _fullName } _fsi:
                    {
                        var xe = _fullName.Split("\\")[a.level - 1];
                        var split = _FileSystemInfo.FullName.Split("\\");
                        if (split.Length < a.level)
                        {
                            return false;
                        }
                        var xe2 = split[a.level - 1];
                        if (xe.Equals(xe2, StringComparison.CurrentCultureIgnoreCase) && a.tree is Tree tree)
                        {
                            if (_fsi.FileSystemInfo.FullName.TrimEnd('\\').Equals(_FileSystemInfo.FullName.TrimEnd('\\'), StringComparison.CurrentCultureIgnoreCase))
                                return true;
                            if (tree.Children.Count() == 0 && a.tree.Value is IYieldItems model && a.tree is INodeViewModel node)
                            {
                                node.IsExpanded = true;
                            }
                        }

                        break;
                    }
            }
            return false;
        }

        #region properties

        public bool ExcludesFiles
        {
            get { return (bool)GetValue(ExcludesFilesProperty); }
            set { SetValue(ExcludesFilesProperty, value); }
        }

        public FileSystemInfo FileSystemInfo
        {
            get { return (FileSystemInfo)GetValue(FileSystemInfoProperty); }
            set { SetValue(FileSystemInfoProperty, value); }
        }

        public IEnumerable Directories
        {
            get { return (IEnumerable)GetValue(DirectoriesProperty); }
            set { SetValue(DirectoriesProperty, value); }
        }

        public bool UseEntryAssembly
        {
            get { return (bool)GetValue(UseEntryAssemblyProperty); }
            set { SetValue(UseEntryAssemblyProperty, value); }
        }

        #endregion properties

        private class CustomItemTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is IGetValue { Value: FileSystemInfo data } tree)
                {
                    return TemplateGenerator.CreateDataTemplate(() =>
                    {
                        var textBlock = new TextBlock { Text = data.Name };
                        return textBlock;
                    });

                    return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Black, Height = 2, Width = 2, VerticalAlignment = VerticalAlignment.Bottom, ToolTip = new ContentControl { Content = data }, Margin = new Thickness(4, 0, 4, 0) });
                }
                else if (item is IFileSystemInfo { FileSystemInfo: { } info })
                {
                    return TemplateGenerator.CreateDataTemplate(() =>
                    {
                        var textBlock = new TextBlock { Text = info.Name };

                        return textBlock;
                    });
                }
                else
                    throw new Exception("d ss!$sd");
            }

            public static CustomItemTemplateSelector Instance { get; } = new();
        }
    }
}