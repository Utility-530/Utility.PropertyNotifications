using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Threading;
using Utility.FileSystem.Transfer.Abstract;
using Utility.FileSystem.Transfer.Common;
using UtilityWpf.Controls.FileSystem;

namespace Utility.FileSystem.Transfer.WPF.Controls
{
    public class FileProgressControl : ProgressControl
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(string), typeof(FileProgressControl), new PropertyMetadata((object)null, PathChanged));
        public static readonly DependencyProperty PathTypeProperty = DependencyProperty.Register(nameof(PathType), typeof(PathType), typeof(FileProgressControl), new PropertyMetadata((object)PathType.Directory));
        private readonly Subject<string> pathChanges = new Subject<string>();
        private PathBrowser fileBrowser;

        public FileProgressControl() => this.pathChanges.CombineLatest<string, Unit, string>((IObservable<Unit>)this.TemplateApplied, (Func<string, Unit, string>)((a, b) => a)).Subscribe<string>((Action<string>)(a => this.fileBrowser?.SetPath.Execute((object)a)));

        public string Path
        {
            get => (string)this.GetValue(FileProgressControl.PathProperty);
            set => this.SetValue(FileProgressControl.PathProperty, (object)value);
        }

        public PathType PathType
        {
            get => (PathType)this.GetValue(FileProgressControl.PathTypeProperty);
            set => this.SetValue(FileProgressControl.PathTypeProperty, (object)value);
        }

        public override void OnApplyTemplate()
        {
            PathType pathType = this.PathType;

            PathBrowser pathBrowser;
            if (pathType != PathType.File)
            {
                if (pathType != PathType.Directory)
                    throw new Exception("555ffffff");
                pathBrowser = new FolderBrowser();
            }
            else
                pathBrowser = new FileBrowser();
            if (true)

                this.fileBrowser = pathBrowser;
            this.fileBrowser.TextChange += ((s, e) => this.Path = e.Text);
            this.ConfigContent ??= (object)this.fileBrowser;
            if (this.Path != null)
                this.pathChanges.OnNext(this.Path);
            base.OnApplyTemplate();
        }

        protected override void ScheduleProgress()
        {
            IObservable<(string, ITransferer)> observable = this.TransferButtonsClicks.WithLatestFrom<Unit, (string, ITransferer), (string, ITransferer)>(this.pathChanges.DistinctUntilChanged<string>().CombineLatest<string, ITransferer, (string, ITransferer)>((IObservable<ITransferer>)this.TransferrerChanges, (Func<string, ITransferer, (string, ITransferer)>)((a, b) => (a, b))), (Func<Unit, (string, ITransferer), (string, ITransferer)>)((a, b) => b));
            observable.CombineLatest<(string, ITransferer), Unit, (string, ITransferer)>((IObservable<Unit>)this.TemplateApplied, (Func<(string, ITransferer), Unit, (string, ITransferer)>)((a, b) => a)).Subscribe<(string, ITransferer)>((Action<(string, ITransferer)>)(a =>
            {
                this.TitleTextBlock.Visibility = Visibility.Visible;
                this.TransferButton.Visibility = Visibility.Collapsed;
            }));
            observable.SelectMany<(string, ITransferer), (DateTime, TimeSpan, Progress)>((Func<(string, ITransferer), IObservable<(DateTime, TimeSpan, Progress)>>)(abc =>
            {
                (string a, ITransferer b) tuple = abc;
                return tuple.b.Transfer(tuple.a).Scan((date: DateTime.Now, new TimeSpan(), new Progress()), ((d, t) => (d.date, DateTime.Now - d.date, t)));
            })).CombineLatest(this.TemplateApplied, ((a, b) => a)).Subscribe(a =>
            {
                var (_, timeSpan2, progress2) = a;
                ((DispatcherObject)this).Dispatcher.Invoke((Action)(() =>
                {
                    this.IsComplete = false;
                    this.ProgressBar.Value = progress2.Percentage;
                    this.ProgressBar.Tag = (object)progress2.Fraction.ToString("00 %");
                    if (progress2.BytesTransferred != progress2.Total)
                        return;
                    this.IsComplete = true;
                    this.RaiseCompleteEvent(timeSpan2);
                    this.TitleTextBlock.Opacity = 0.5;
                }));
            });
        }

        private static void PathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FileProgressControl fileProgressControl))
                return;
            fileProgressControl.pathChanges.OnNext(e.NewValue as string);
        }
    }
}
