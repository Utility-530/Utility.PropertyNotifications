using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace Utility.WPF.Controls.FileSystem
{
    public class DirectoryComboBox : ComboBox
    {
        public static readonly DependencyProperty ConnectionDirectoryProperty = DependencyProperty.Register("ConnectionDirectory", typeof(string), typeof(DirectoryComboBox), new PropertyMetadata(null));
        public static readonly DependencyProperty FileFilterProperty = DependencyProperty.Register("FileFilter", typeof(string), typeof(DirectoryComboBox), new PropertyMetadata("*"));
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(FileInfo), typeof(DirectoryComboBox), new PropertyMetadata(null));
        public static readonly RoutedEvent FileChangeEvent = EventManager.RegisterRoutedEvent("FileChange", RoutingStrategy.Bubble, typeof(FileChangeEventHandler), typeof(DirectoryComboBox));

        //static DirectoryComboBox() {
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryComboBox), new FrameworkPropertyMetadata(typeof(DirectoryComboBox)));
        //}

        public DirectoryComboBox()
        {
            SelectedIndex = 0;

            this.WhenAnyValue(a => a.ConnectionDirectory)
                .Where(a => a != null)
                .Select(a => new DirectoryInfo(a))
                .Where(a => a != null)
                .CombineLatest(this.WhenAnyValue(a => a.FileFilter))
                .Subscribe(a =>
                {
                    try
                    {
                        var (first, second) = a;
                        var files = first.GetFiles(second).ToArray();
                        this.ItemsSource = files.Select(fileInfo => fileInfo.FullName).ToArray();
                    }
                    catch (Exception e)
                    {
                        this.ItemsSource = new[] { e.Message };
                    }
                });

            this.Events()
                .SelectionChanged
                .SelectMany(a => a.AddedItems.Cast<string>())
                .Where(a => a != null && IsValidPath(a))
                .Select(a => new FileInfo(a))
                .Subscribe(fileInfo =>
                {
                    RaiseEvent(new FileChangeEventArgs(FileChangeEvent, fileInfo));
                    Output = fileInfo;
                });

            static bool IsValidPath(string path, bool allowRelativePaths = false)
            {
                try
                {
                    return allowRelativePaths ?
                        Path.IsPathRooted(path) :
                        string.IsNullOrEmpty(Path.GetPathRoot(path)?.Trim('\\', '/')) == false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public string ConnectionDirectory
        {
            get => (string)GetValue(ConnectionDirectoryProperty);
            set => SetValue(ConnectionDirectoryProperty, value);
        }

        public string FileFilter
        {
            get => (string)GetValue(FileFilterProperty);
            set => SetValue(FileFilterProperty, value);
        }

        // public DirectoryInfo Output

        public FileInfo Output
        {
            get => (FileInfo)GetValue(OutputProperty);
            set => SetValue(OutputProperty, value);
        }

        public event FileChangeEventHandler FileChange
        {
            add => AddHandler(FileChangeEvent, value);
            remove => RemoveHandler(FileChangeEvent, value);
        }

        public delegate void FileChangeEventHandler(object sender, FileChangeEventArgs args);

        public class FileChangeEventArgs : RoutedEventArgs
        {
            public FileChangeEventArgs(RoutedEvent routedEvent, FileInfo fileInfo) : base(routedEvent)
            {
                FileInfo = fileInfo;
            }

            public FileInfo FileInfo { get; }
        }
    }
}