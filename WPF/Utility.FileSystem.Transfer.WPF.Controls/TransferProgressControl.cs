using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Abstract;
using UtilityWpf.Controls.FileSystem;

namespace Utility.FileSystem.Transfer.WPF.Controls
{
    public class TransferProgressControl : ProgressControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Path", typeof(string), typeof(TransferProgressControl), new PropertyMetadata((object)null, SourceChanged));
        public static readonly DependencyProperty DestinationProperty = DependencyProperty.Register(nameof(Destination), typeof(string), typeof(TransferProgressControl), new PropertyMetadata((object)null,  DestinationChanged));
        public static readonly DependencyProperty SourceTypeProperty = DependencyProperty.Register("PathType", typeof(PathType), typeof(TransferProgressControl), new PropertyMetadata((object)PathType.Directory));
        public static readonly DependencyProperty DestinationTypeProperty = DependencyProperty.Register(nameof(DestinationType), typeof(PathType), typeof(TransferProgressControl), new PropertyMetadata((object)PathType.Directory));
        private readonly Subject<string> destinationChanges = new Subject<string>();
        private readonly Subject<string> sourceChanges = new Subject<string>();
        private PathBrowser browseDestination;
        private PathBrowser browseSource;

        public TransferProgressControl()
        {
            this.sourceChanges.CombineLatest(this.TemplateApplied, (a, b) => a).DistinctUntilChanged<string>().Subscribe(a => this.browseSource.SetPath.Execute(a));
            this.destinationChanges.CombineLatest(this.TemplateApplied, (a, b) => a).DistinctUntilChanged<string>().Subscribe(a => this.browseDestination.SetPath.Execute(a));
        }

        public string Source
        {
            get => (string)this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, (object)value);
        }

        public string Destination
        {
            get => (string)this.GetValue(DestinationProperty);
            set => this.SetValue(DestinationProperty, (object)value);
        }

        public PathType SourceType
        {
            get => (PathType)this.GetValue(SourceTypeProperty);
            set => this.SetValue(SourceTypeProperty, (object)value);
        }

        public PathType DestinationType
        {
            get => (PathType)this.GetValue(DestinationTypeProperty);
            set => this.SetValue(DestinationTypeProperty, (object)value);
        }

        public override void OnApplyTemplate()
        {
            StackPanel stackPanel = new StackPanel();
            this.browseSource = this.SourceType == PathType.File ? new FileBrowser() : new FolderBrowser();
            this.browseDestination = this.DestinationType == PathType.File ? new FileBrowser() : new FolderBrowser();
            this.browseSource.TextChange += ((s, e) => this.sourceChanges.OnNext(e.Text));
            this.browseDestination.TextChange += ((s, e) => this.destinationChanges.OnNext(e.Text));
            if (this.ConfigContent == null)
            {
                object obj;
                this.ConfigContent = (StackPanel)(obj = stackPanel);
            }
            if (this.Source != null)
                this.sourceChanges.OnNext(this.Source);
            if (this.Destination != null)
                this.destinationChanges.OnNext(this.Destination);
            base.OnApplyTemplate();
        }

        protected override void ScheduleProgress()
        {
            IObservable<(string, string, ITransferer)> observable = this.TransferButtonsClicks.WithLatestFrom(this.sourceChanges.DistinctUntilChanged<string>()
                .CombineLatest(this.destinationChanges.DistinctUntilChanged(), this.TransferrerChanges, (a, b, c) => (a, b, c)),  (a, b) => b);
            observable.CombineLatest(this.TemplateApplied, (a, _) => a).Subscribe(a =>
            {
                this.TitleTextBlock.Visibility = Visibility.Visible;
                this.TransferButton.Visibility = Visibility.Collapsed;
            });
            observable.SelectMany(abc =>
            {
                (string a, string b, ITransferer c) tuple = abc;
                return tuple.c.Transfer(tuple.a, tuple.b).Scan((date: DateTime.Now, new TimeSpan(), new Progress()), (d, t) => (d.date, DateTime.Now - d.date, t));
            }).CombineLatest(this.TemplateApplied, (a, b) => a).Subscribe((a =>
            {
                (DateTime _, TimeSpan timeSpan2, Progress progress2) = a;
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
            }));
        }

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as TransferProgressControl)?.sourceChanges.OnNext(e.NewValue as string);

        private static void DestinationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as TransferProgressControl)?.destinationChanges.OnNext(e.NewValue as string);
    }
}
