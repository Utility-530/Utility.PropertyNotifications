using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Controls.FileSystem
{
    using System;
    using System.Reactive.Subjects;

    public class FolderOpenControl : Control
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(FolderOpenControl), new PropertyMetadata(null, PathChanged));
        public static readonly DependencyProperty FolderOpenCommandProperty = DependencyProperty.Register("FolderOpenCommand", typeof(ICommand), typeof(FolderOpenControl));

        private static void PathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        static FolderOpenControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderOpenControl), new FrameworkPropertyMetadata(typeof(FolderOpenControl)));
        }

        public FolderOpenControl()
        {
            var foc = new FolderOpenCommand();
            SetValue(FolderOpenCommandProperty, foc);

            foc.Subscribe(directory =>
            {
                Dispatcher.InvokeAsync(() => Path = directory);
            });
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        private class FolderOpenCommand : /*NPC,*/ ICommand, IObservable<string>
        {
            private string directory;
            private ReplaySubject<string> directoryChanges = new(1);

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            //public string Directory
            //{
            //    get { return directory; }
            //    set { OnPropertyChanged(ref directory, value); }
            //}

            public void Execute(object parameter)
            {
                using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        directoryChanges.OnNext(fbd.SelectedPath);
                }
            }

            public IDisposable Subscribe(IObserver<string> observer)
            {
                return directoryChanges.Subscribe(observer);
            }
        }
    }
}