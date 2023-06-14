using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.WPF.Controls.FileSystem.Infrastructure;
using Button = System.Windows.Controls.Button;
using Control = System.Windows.Controls.Control;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;

namespace Utility.WPF.Controls.FileSystem
{
    /// <summary>
    /// Interaction logic for PathBrowser.xaml
    /// </summary>
    public abstract class PathBrowser<T> : PathBrowser where T : FrameworkElement
    {
        protected readonly ReplaySubject<T> textBoxContentChanges = new(1);

        public static readonly DependencyProperty TextBoxContentProperty =
            DependencyProperty.Register("TextBoxContent", typeof(object), typeof(PathBrowser<T>), new PropertyMetadata(null, TextBoxContentChanged));

        public PathBrowser()
        {
            applyTemplateSubject
                .CombineLatest(textBoxContentChanges, (a, b) => b)
                .Subscribe(content =>
                {
                    (ContentControlOne ?? throw new NullReferenceException("ContentControlOne is null")).Content = content;
                    //(content as DependencyObject ?? throw new NullReferenceException("content is null"))
                    //    .FindChildren<TextBox>()
                    //    .ToObservable()
                    //    .Merge(Observable.Return(content as TextBox).WhereNotNull())
                    //    .Subscribe(textBoxSubject.OnNext);
                });

            SetPath = ReactiveCommand.Create<string>(textChanges.OnNext);

            Command.TextChanged += Command_TextChanged;

            textChanges
                .WhereNotNull()
                .DistinctUntilChanged()
                .CombineLatest(applyTemplateSubject, (a, b) => a)
                .WithLatestFrom(textBoxContentChanges)
                .SubscribeOnDispatcher()
                .Subscribe(a =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        var (path, textBox) = a;
                        OnTextChange(path, textBox);
                    });
                });
        }

        protected virtual void OnTextChange(string path, T sender)
        {
            RaiseTextChangeEvent(path);
        }

        private void Command_TextChanged(string obj)
        {
            textChanges.OnNext(obj);
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

        public object TextBoxContent
        {
            get => GetValue(TextBoxContentProperty);
            set => SetValue(TextBoxContentProperty, value);
        }

        private static void TextBoxContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PathBrowser<T> ?? throw new NullReferenceException("PathBrowser is null")).textBoxContentChanges.OnNext((T)e.NewValue);
        }
    }

    /// <summary>
    /// Interaction logic for PathBrowser.xaml
    /// </summary>
    public abstract class PathBrowser : Control
    {
        protected readonly ReplaySubject<string> textChanges = new(1);
        protected readonly ReplaySubject<Unit> applyTemplateSubject = new(1);

        protected Button ButtonOne;
        protected ContentControl ContentControlOne;
        protected Label LabelOne;
        protected TextBox TextBoxOne;

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
            SetPath = ReactiveCommand.Create<string>(textChanges.OnNext);

            Command.TextChanged += Command_TextChanged;
        }

        private void Command_TextChanged(string obj)
        {
            textChanges.OnNext(obj);
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
            //var textBox = sender ?? throw new NullReferenceException("TextBoxContent is null");
            textBox.Text = path;
            textBox.Focus();
            var length = System.IO.Path.GetFileName(path).Length;
            textBox.Select(path.Length - length, length);
            textBox.ToolTip = path;
        }

        public static void OnTextChange(string path, TextBlock textBox)
        {
            //var textBox = sender ?? throw new NullReferenceException("TextBoxContent is null");
            textBox.Text = path;
            textBox.Focus();
            var length = System.IO.Path.GetFileName(path).Length;
            textBox.ToolTip = path;
        }
    }
}