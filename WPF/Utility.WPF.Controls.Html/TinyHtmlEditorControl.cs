using System.IO;
using System.Windows;
using System.Windows.Controls;
using AvalonEditB;
using AvalonEditB.Highlighting;
using TinyHtml.Wpf;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls.Html
{
    public class TinyHtmlEditorControl : SplitControl
    {
        private HtmlControl? _htmlControl;
        private TextEditor? _textEditor;
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(TinyHtmlEditorControl), new PropertyMetadata(changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TinyHtmlEditorControl { _textEditor: { } cntr } c && e.NewValue is string s)
            {
                cntr.Text = s;
            }
        }

        static TinyHtmlEditorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TinyHtmlEditorControl), new FrameworkPropertyMetadata(typeof(TinyHtmlEditorControl)));
        }

        public TinyHtmlEditorControl()
        {
            this.Loaded += TinyHtmlEditorControl_Loaded;
            this.Unloaded += TinyHtmlEditorControl_Unloaded;
        }

        private void TinyHtmlEditorControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.RemoveLogicalChild(this.Content);
            this.Content = null;
            _htmlControl = null;

            this.RemoveLogicalChild(this.Header);
            this.Header = null;
            _textEditor = null;
        }

        private void TinyHtmlEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            combine();
        }

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            //this.Movement = Enums.XYMovement.TopToBottom;
            //_htmlControl = this.Content as WpfHtmlControl;
            _htmlControl = this.Content as HtmlControl;
            _htmlControl.Html = Html;
            //_htmlControl.Html = Html;
            // its necessary to set the header in code-behind because of the difficulty detaching the TextEditor
            // when control is unloaded
            this.Header ??= new TextEditor()
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                Text = Html,
                BorderThickness = new Thickness(),
                SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML"),
                ShowLineNumbers = true
            };
            _textEditor = this.Header as TextEditor;
            if (_textEditor != null)

                _textEditor.TextChanged += _textEditor_TextChanged;
            base.OnApplyTemplate();
        }

        private void _textEditor_TextChanged(object? sender, EventArgs e)
        {
            _htmlControl.Html = GetFixedHtml();
        }

        private string GetFixedHtml()
        {
            var html = GetHtmlEditorText();

            //html = Regex.Replace(html, @"src=\""(\w.*?)\""", match =>
            //{
            //    var img = HtmlRenderingHelper.TryLoadResourceImage(match.Groups[1].Value);
            //    if (img != null)
            //    {
            //        var tmpFile = Path.GetTempFileName();
            //        var encoder = new PngBitmapEncoder();
            //        encoder.Frames.Add(BitmapFrame.Create(img));
            //        using (FileStream stream = new FileStream(tmpFile, FileMode.Create))
            //            encoder.Save(stream);
            //        return string.Format("src=\"{0}\"", tmpFile);
            //    }
            //    return match.Value;
            //}, RegexOptions.IgnoreCase);

            //html = Regex.Replace(html, @"href=\""(\w.*?)\""", match =>
            //{
            //    var stylesheet = DemoUtils.GetStylesheet(match.Groups[1].Value);
            //    if (stylesheet != null)
            //    {
            //        var tmpFile = Path.GetTempFileName();
            //        File.WriteAllText(tmpFile, stylesheet);
            //        return string.Format("href=\"{0}\"", tmpFile);
            //    }
            //    return match.Value;
            //}, RegexOptions.IgnoreCase);

            return html;
        }

        private string GetHtmlEditorText()
        {
            //return new TextRange(_htmlEditor.Document.ContentStart, _htmlEditor.Document.ContentEnd).Text;
            return _textEditor?.Document.Text ?? string.Empty;
        }

        private void combine()
        {
            string cssFile = "sakura.css";
            //string cssFile = "pico.min.css";
            // string cssFile = "entireframework.min.css";
            //string cssFile = "pico.classless.blue.css";

            string name = typeof(TinyHtmlControl).Assembly.GetManifestResourceNames().Where(a => a.Contains(cssFile)).First();

            using (var s = typeof(TinyHtmlControl).Assembly.GetManifestResourceStream(name))
            {
                WpfHtmlControlBase.SetMasterStylesheet(new StreamReader(s).ReadToEnd());
            }
        }
    }
}