using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Abstract;
using Utility.FileSystem.Transfer.WPF.Controls.Abstract;
using UtilityWpf;

namespace Utility.FileSystem.Transfer.WPF.Controls
{
    public class ProgressControl : Control, IProgressView
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ProgressControl), new PropertyMetadata((object)"Progressing", TitleChanged));
        public static readonly DependencyProperty DetailsProperty = DependencyProperty.Register(nameof(Details), typeof(string), typeof(ProgressControl), new PropertyMetadata((object)"Details about task", DetailsChanged));
        public static readonly DependencyProperty TransfererProperty = DependencyProperty.Register(nameof(Transferer), typeof(ITransferer), typeof(ProgressControl), new PropertyMetadata(TransfererChanged));
        public static readonly DependencyProperty IsCompleteProperty = DependencyProperty.Register(nameof(IsComplete), typeof(bool), typeof(ProgressControl), new PropertyMetadata((object)false));
        public static readonly RoutedEvent CompleteEvent = EventManager.RegisterRoutedEvent("Complete", RoutingStrategy.Bubble, typeof(MultiProgress.TimeSpanRoutedEventHandler), typeof(ProgressControl));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(ProgressControl), new PropertyMetadata(readOnlyChanged));
        public static readonly DependencyProperty RunCommandProperty = DependencyProperty.Register(nameof(RunCommand), typeof(ICommand), typeof(ProgressControl), new PropertyMetadata(null));
        public static readonly DependencyProperty ShowTransferProperty = DependencyProperty.Register(nameof(ShowTransfer), typeof(bool), typeof(ProgressControl), new PropertyMetadata(showTransferChanged));
        public static readonly DependencyProperty ConfigContentProperty = DependencyProperty.Register(nameof(ConfigContent), typeof(object), typeof(ProgressControl), new PropertyMetadata(ConfigContentChanged));
        protected readonly Subject<object> ConfigContentControlChanges = new Subject<object>();
        protected readonly Subject<string> DetailChanges = new Subject<string>();
        protected readonly Subject<bool> ReadOnlyChanges = new Subject<bool>();
        private readonly ReplaySubject<TimeSpanRoutedEventArgs> replaySubject = new ReplaySubject<TimeSpanRoutedEventArgs>(1);
        protected readonly Subject<bool> ShowTransferChanges = new Subject<bool>();
        protected readonly Subject<Unit> TemplateApplied = new Subject<Unit>();
        protected readonly Subject<string> TitleChanges = new Subject<string>();
        protected readonly Subject<Unit> TransferButtonsClicks = new Subject<Unit>();
        protected readonly Subject<ITransferer> TransferrerChanges = new Subject<ITransferer>();
        protected ContentControl ConfigContentControl;
        protected ProgressBar ProgressBar;
        protected TextBlock TitleTextBlock;
        protected Panel TopPanel;
        protected Button TransferButton;

        static ProgressControl() => FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(ProgressControl)));

        public ProgressControl()
        {
            this.RunCommand = (ICommand)ReactiveCommand.Create((Action)(() => this.TransferButtonsClicks.OnNext(Unit.Default)));
            this.ConfigContentControlChanges.CombineLatest<object, Unit, object>((IObservable<Unit>)this.TemplateApplied, (Func<object, Unit, object>)((a, b) => a)).Subscribe<object>((Action<object>)(content => this.ConfigContentControl.Content = content));
            this.ShowTransferChanges.CombineLatest<bool, Unit, bool>((IObservable<Unit>)this.TemplateApplied, (Func<bool, Unit, bool>)((a, b) => a)).SubscribeOnDispatcher<bool>().Subscribe<bool>((Action<bool>)(a => this.TransferButton.Visibility = a ? Visibility.Visible : Visibility.Hidden));
            this.ReadOnlyChanges.CombineLatest<bool, Unit, bool>((IObservable<Unit>)this.TemplateApplied, (Func<bool, Unit, bool>)((a, b) => a)).Subscribe<bool>((Action<bool>)(a =>
            {
                this.TransferButton.Visibility = a ? Visibility.Collapsed : Visibility.Visible;
                this.ConfigContentControl.Visibility = a ? Visibility.Collapsed : Visibility.Visible;
            }));
            this.DetailChanges.CombineLatest<string, Unit, string>((IObservable<Unit>)this.TemplateApplied, (Func<string, Unit, string>)((a, b) => a)).Subscribe<string>((Action<string>)(a => this.TitleTextBlock.ToolTip = (object)a));
            this.TitleChanges.CombineLatest(this.LoadedChanges()).Subscribe(a => this.TitleTextBlock.Text = a.First);
            this.ScheduleProgress();
            this.LoadedChanges()
            .Subscribe(a =>
            {
                this.ReadOnlyChanges.OnNext(this.IsReadOnly);
                this.ShowTransferChanges.OnNext(this.ShowTransfer);
            });
        }

        public string Title
        {
            get => (string)this.GetValue(ProgressControl.TitleProperty);
            set => this.SetValue(ProgressControl.TitleProperty, (object)value);
        }

        public string Details
        {
            get => (string)this.GetValue(ProgressControl.DetailsProperty);
            set => this.SetValue(ProgressControl.DetailsProperty, (object)value);
        }

        public ITransferer Transferer
        {
            get => (ITransferer)this.GetValue(ProgressControl.TransfererProperty);
            set => this.SetValue(ProgressControl.TransfererProperty, (object)value);
        }

        public bool IsComplete
        {
            get => (bool)this.GetValue(ProgressControl.IsCompleteProperty);
            set => this.SetValue(ProgressControl.IsCompleteProperty, (object)value);
        }

        public bool IsReadOnly
        {
            get => (bool)this.GetValue(ProgressControl.IsReadOnlyProperty);
            set => this.SetValue(ProgressControl.IsReadOnlyProperty, (object)value);
        }

        public bool ShowTransfer
        {
            get => (bool)this.GetValue(ProgressControl.ShowTransferProperty);
            set => this.SetValue(ProgressControl.ShowTransferProperty, (object)value);
        }

        public object ConfigContent
        {
            get => this.GetValue(ProgressControl.ConfigContentProperty);
            set => this.SetValue(ProgressControl.ConfigContentProperty, value);
        }

        public IObservable<TimeSpan> CompleteEvents => this.replaySubject.AsObservable<TimeSpanRoutedEventArgs>().Select<TimeSpanRoutedEventArgs, TimeSpan>((Func<TimeSpanRoutedEventArgs, TimeSpan>)(a => a.TimeSpan));

        public ICommand RunCommand
        {
            get => (ICommand)this.GetValue(ProgressControl.RunCommandProperty);
            set => this.SetValue(ProgressControl.RunCommandProperty, (object)value);
        }

        public override void OnApplyTemplate()
        {
            this.TransferButton = this.GetTemplateChild("transferButton") as Button;
            this.TopPanel = this.GetTemplateChild("TopPanel") as Panel;
            this.TransferButton = this.GetTemplateChild("transferButton") as Button;
            this.ConfigContentControl = this.GetTemplateChild("ContentControl1") as ContentControl;
            this.TransferButton.Command = this.RunCommand;
            this.TitleTextBlock = this.GetTemplateChild("TitleTextBlock") as TextBlock;
            this.TitleTextBlock.Text = this.Title;
            this.ProgressBar = this.GetTemplateChild("progressBar") as ProgressBar;
            base.OnApplyTemplate();
            this.TemplateApplied.OnNext(Unit.Default);
        }

        protected virtual void ScheduleProgress()
        {
            IObservable<ITransferer> source = this.TransferButtonsClicks.CombineLatest<Unit, Unit, Unit>((IObservable<Unit>)this.TemplateApplied, (Func<Unit, Unit, Unit>)((a, b) => a)).WithLatestFrom<Unit, ITransferer, ITransferer>((IObservable<ITransferer>)this.TransferrerChanges, (Func<Unit, ITransferer, ITransferer>)((a, b) => b));
            source.Select(transferer => transferer.Transfer().Scan((date:DateTime.Now, new TimeSpan(), new Progress()),(d, t) => (d.date, DateTime.Now - d.date, t))).Switch<(DateTime, TimeSpan, Progress)>().Subscribe<(DateTime, TimeSpan, Progress)>((Action<(DateTime, TimeSpan, Progress)>)(a =>
            {
                (DateTime _, TimeSpan timeSpan2, Progress progress2) = a;
                (this).Dispatcher.Invoke(() =>
                {
                    this.IsComplete = false;
                    this.ProgressBar.Value = progress2.Percentage;
                    this.ProgressBar.Tag = (object)progress2.Fraction.ToString("00 %");
                    if (progress2.BytesTransferred != progress2.Total)
                        return;
                    this.IsComplete = true;
                    this.RaiseCompleteEvent(timeSpan2);
                    this.TitleTextBlock.Opacity = 0.5;
                });
            }));
            source.Subscribe<ITransferer>((Action<ITransferer>)(a =>
            {
                this.TitleTextBlock.Visibility = Visibility.Visible;
                this.TransferButton.Visibility = Visibility.Collapsed;
            }));
        }

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
                return;
            progressControl.TitleChanges.OnNext((string)e.NewValue);
        }

        private static void DetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
                return;
            progressControl.DetailChanges.OnNext((string)e.NewValue);
        }

        private static void TransfererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
                return;
            progressControl.TransferrerChanges.OnNext(e.NewValue as ITransferer);
        }

        private static void readOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
                return;
            progressControl.ReadOnlyChanges.OnNext((bool)((DependencyPropertyChangedEventArgs)e).NewValue);
        }

        public event MultiProgress.TimeSpanRoutedEventHandler Complete
        {
            add => this.AddHandler(ProgressControl.CompleteEvent, (Delegate)value);
            remove => this.RemoveHandler(ProgressControl.CompleteEvent, (Delegate)value);
        }

        protected void RaiseCompleteEvent(TimeSpan timeSpan)
        {
            TimeSpanRoutedEventArgs spanRoutedEventArgs = new TimeSpanRoutedEventArgs(ProgressControl.CompleteEvent, timeSpan);
            this.RaiseEvent((RoutedEventArgs)spanRoutedEventArgs);
            this.replaySubject.OnNext(spanRoutedEventArgs);
        }

        private static void showTransferChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
                return;
            progressControl.ShowTransferChanges.OnNext((bool)e.NewValue);
        }

        private static void ConfigContentChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ProgressControl progressControl))
                return;
            progressControl.ConfigContentControlChanges.OnNext(e.NewValue);
        }
    }
}
