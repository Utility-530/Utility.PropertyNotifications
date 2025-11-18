using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.WPF.Reactives;
using Utility.WPF.Controls.FileSystem.Infrastructure;
using Button = System.Windows.Controls.Button;
using Control = System.Windows.Controls.Control;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;
using Utility.Commands;
using Utility.Reactives;

namespace Utility.WPF.Controls.FileSystem
{
    /// <summary>
    /// Interaction logic for PathBrowser.xaml
    /// </summary>
    public abstract class PathBrowser<T> : PathBrowser where T : FrameworkElement
    {
        protected readonly ReplaySubject<T> contentChanges = new(1);

        public static readonly DependencyProperty EditContentProperty =
            DependencyProperty.Register("EditContent", typeof(object), typeof(PathBrowser<T>), new PropertyMetadata(null, EditContentChanged));

        public PathBrowser()
        {
            applyTemplateSubject
                .CombineLatest(contentChanges, (a, b) => b)
                .Subscribe(content =>
                {
                    (ContentControlOne ?? throw new NullReferenceException("ContentControlOne is null")).Content = content;
                });

            SetPath = new Command<string>(pathChanges.OnNext);

            pathChanges
                .WhereIsNotNull()
                .DistinctUntilChanged()
                .CombineLatest(applyTemplateSubject, (a, b) => a)
                .WithLatestFrom(contentChanges)
                .SubscribeOn(SynchronizationContextScheduler.Instance)
                .Subscribe(a =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        var (path, element) = a;
                        OnPathChange(path, element);
                    });
                });
        }

        protected virtual void OnPathChange(string path, T sender)
        {
            RaiseTextChangeEvent(path);
            Path = path;
        }

        public override void OnApplyTemplate()
        {
            ButtonOne = GetTemplateChild("ButtonOne") as Button ?? throw new NullReferenceException("ButtonOne is null");
            ButtonOne.Command = Command;
            ContentControlOne = GetTemplateChild("ContentControlOne") as ContentControl ?? throw new NullReferenceException("ContentControlOne is null");
            LabelOne = GetTemplateChild("LabelOne") as Label ?? throw new NullReferenceException("LabelOne is null");
            LabelOne.Content = Label;
            base.OnApplyTemplate();
            applyTemplateSubject.OnNext(Unit.Default);
        }

        public object EditContent
        {
            get => GetValue(EditContentProperty);
            set => SetValue(EditContentProperty, value);
        }

        private static void EditContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PathBrowser<T> ?? throw new NullReferenceException("PathBrowser is null")).contentChanges.OnNext((T)e.NewValue);
        }
    }

    /// <summary>
    /// Interaction logic for PathBrowser.xaml
    /// </summary>
    public abstract class PathBrowser : Control
    {
        protected readonly ReplaySubject<string> pathChanges = new(1);
        protected readonly ReplaySubject<Unit> applyTemplateSubject = new(1);

        protected Button ButtonOne;
        protected ContentControl ContentControlOne;
        protected Label LabelOne;
        protected TextBox TextBoxOne;

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(PathBrowser), new PropertyMetadata(onPathChanged));

        private static void onPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PathBrowser pathBrowser && e.NewValue is string str)
                pathBrowser.pathChanges.OnNext(str);
        }

        public static readonly DependencyProperty SetPathProperty =
            DependencyProperty.Register("SetPath", typeof(ICommand), typeof(PathBrowser), new PropertyMetadata(null));

        public static readonly RoutedEvent TextChangeEvent = EventManager.RegisterRoutedEvent("TextChange", RoutingStrategy.Bubble, typeof(TextChangedRoutedEventHandler), typeof(PathBrowser));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(PathBrowser), new PropertyMetadata(null));

        static PathBrowser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathBrowser), new FrameworkPropertyMetadata(typeof(PathBrowser)));
        }

        public PathBrowser()
        {
            SetPath = new Command<string>(pathChanges.OnNext);
            Command.TextChanged += Command_TextChanged;
        }

        private void Command_TextChanged(string path)
        {
            pathChanges.OnNext(path);
            Path = path;
        }

        public override void OnApplyTemplate()
        {
            ButtonOne = GetTemplateChild("ButtonOne") as Button ?? throw new NullReferenceException("ButtonOne is null");
            ButtonOne.Command = Command;
            ContentControlOne = GetTemplateChild("ContentControlOne") as ContentControl ?? throw new NullReferenceException("ContentControlOne is null");
            LabelOne = GetTemplateChild("LabelOne") as Label ?? throw new NullReferenceException("LabelOne is null");
            LabelOne.Content = Label;
            base.OnApplyTemplate();
            applyTemplateSubject.OnNext(Unit.Default);
        }

        #region properties

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public ICommand SetPath
        {
            get => (ICommand)GetValue(SetPathProperty);
            set => SetValue(SetPathProperty, value);
        }

        public event TextChangedRoutedEventHandler TextChange
        {
            add => AddHandler(TextChangeEvent, value);
            remove => RemoveHandler(TextChangeEvent, value);
        }

        #endregion properties

        protected abstract BrowserCommand Command { get; }

        protected void RaiseTextChangeEvent(string text)
        {
            Dispatcher.Invoke(() => RaiseEvent(new TextChangedRoutedEventArgs(TextChangeEvent, text)));
        }
    }

    internal static class Helper
    {
        public static void OnTextChange(string path, TextBox textBox)
        {
            //var textBox = sender ?? throw new NullReferenceException("EditContent is null");
            textBox.Text = path;
            textBox.Focus();
            var length = System.IO.Path.GetFileName(path).Length;
            textBox.Select(path.Length - length, length);
            textBox.ToolTip = path;
        }

        public static void OnTextChange(string path, TextBlock textBox)
        {
            //var textBox = sender ?? throw new NullReferenceException("EditContent is null");
            textBox.Text = path;
            textBox.Focus();
            var length = System.IO.Path.GetFileName(path).Length;
            textBox.ToolTip = path;
        }
    }
}