using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignExtensions.Controls;
using Microsoft.Xaml.Behaviors.Core;
using ReactiveUI;

namespace Utility.WPF.Controls.FileSystem
{
    public class FileViewer : Control
    {
        public static readonly DependencyProperty DirectoryProperty = DependencyProperty.Register("Directory", typeof(string), typeof(FileViewer), new PropertyMetadata(null));
        public static readonly DependencyProperty RefreshProperty = DependencyProperty.Register("Refresh", typeof(ICommand), typeof(FileViewer), new PropertyMetadata(null));

        private OpenFileControl openFileControl;
        private ContentControl contentControl;

        static FileViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileViewer), new FrameworkPropertyMetadata(typeof(FileViewer)));
        }

        public FileViewer()
        {
        }

        public override void OnApplyTemplate()
        {
            openFileControl = GetTemplateChild("OpenFileControl1") as OpenFileControl ?? throw new NullReferenceException("OpenFileControl1");
            contentControl = GetTemplateChild("ContentControl1") as ContentControl ?? throw new NullReferenceException("ContentControl1");

            this.WhenAnyValue(v => v.Directory)
                .Subscribe(directory =>
            {
                openFileControl.CurrentDirectory = directory;
            });

            openFileControl.FileSelected += OpenFileControl_FileSelected;

            Refresh = new ActionCommand(() =>
            {
                string directory = Directory;
                Directory = "c:\\";
                Directory = directory;
            });
        }

        #region properties

        public string Directory
        {
            get => (string)GetValue(DirectoryProperty);
            set => SetValue(DirectoryProperty, value);
        }

        public ICommand Refresh
        {
            get => (ICommand)GetValue(RefreshProperty);
            set => SetValue(RefreshProperty, value);
        }

        #endregion properties

        protected virtual void OpenFileControl_FileSelected(object sender, RoutedEventArgs e)
        {
            if (e is not FileSelectedEventArgs fileSelectedEventArgs) return;

            switch (Path.GetExtension(fileSelectedEventArgs.File))
            {
                case ".json":
                    contentControl.Content = ConvertJson(fileSelectedEventArgs.File);
                    break;

                case ".csv":
                    contentControl.Content = ConvertCsv(fileSelectedEventArgs.File);
                    break;

                case ".png":
                    var bitmap = new ImageSourceConverter().ConvertFromString(fileSelectedEventArgs.File);
                    contentControl.Content = new Image { Source = (ImageSource)bitmap };
                    break;

                case ".pdf":
                    contentControl.Content = ConvertPdf(fileSelectedEventArgs.File);
                    break;
            }
        }

        protected virtual object ConvertCsv(string filePath)
        {
            throw new NotImplementedException();
        }

        protected virtual object ConvertJson(string filePath)
        {
            var jsonView = new TextBlock { Text = File.ReadAllText(filePath) };
            return jsonView;
        }

        protected virtual object ConvertPdf(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}