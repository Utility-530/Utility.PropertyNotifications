using AvalonEditB;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using TheArtOfDev.HtmlRenderer.Demo.WPF;
using TinyHtml.Wpf;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls.Html
{
    public class TinyHtmlEditorControl : SplitControl
    {
        private WpfHtmlControl? _htmlControl;
        private TextEditor? _textEditor;
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(TinyHtmlEditorControl), new PropertyMetadata(changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        static TinyHtmlEditorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TinyHtmlEditorControl), new FrameworkPropertyMetadata(typeof(TinyHtmlEditorControl)));
        }

        public TinyHtmlEditorControl()
        {
            this.Loaded += TinyHtmlEditorControl_Loaded;
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
            _htmlControl = this.Content as WpfHtmlControl;
            _textEditor = this.Header as TextEditor;
            _textEditor.TextChanged += _textEditor_TextChanged;
            base.OnApplyTemplate();
        }

        private void _textEditor_TextChanged(object? sender, EventArgs e)
        {
            _htmlControl.Html = GetFixedHtml();
        }

        string GetFixedHtml()
        {
            var html = GetHtmlEditorText();

            html = Regex.Replace(html, @"src=\""(\w.*?)\""", match =>
            {
                var img = HtmlRenderingHelper.TryLoadResourceImage(match.Groups[1].Value);
                if (img != null)
                {
                    var tmpFile = Path.GetTempFileName();
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(img));
                    using (FileStream stream = new FileStream(tmpFile, FileMode.Create))
                        encoder.Save(stream);
                    return string.Format("src=\"{0}\"", tmpFile);
                }
                return match.Value;
            }, RegexOptions.IgnoreCase);

            html = Regex.Replace(html, @"href=\""(\w.*?)\""", match =>
            {
                var stylesheet = DemoUtils.GetStylesheet(match.Groups[1].Value);
                if (stylesheet != null)
                {
                    var tmpFile = Path.GetTempFileName();
                    File.WriteAllText(tmpFile, stylesheet);
                    return string.Format("href=\"{0}\"", tmpFile);
                }
                return match.Value;
            }, RegexOptions.IgnoreCase);

            return html;
        }

        string GetHtmlEditorText()
        {
            //return new TextRange(_htmlEditor.Document.ContentStart, _htmlEditor.Document.ContentEnd).Text;
            return _textEditor.Document.Text;
        }

        void combine()
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
