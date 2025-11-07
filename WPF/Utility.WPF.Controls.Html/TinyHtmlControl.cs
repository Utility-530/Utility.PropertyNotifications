using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TinyHtml.Wpf;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls.Html
{
    public class TinyHtmlControl : SplitControl, IHtmlControl
    {
        private bool _updateLock;
        private WpfHtmlControl? _htmlPanel;
        private System.Windows.Controls.WebBrowser _webBrowser;

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(TinyHtmlControl), new PropertyMetadata(changed));

        static TinyHtmlControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TinyHtmlControl), new FrameworkPropertyMetadata(typeof(TinyHtmlControl)));
        }

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TinyHtmlControl view && e.NewValue is string s)
            {
                view.Update(s);
                view.combine();
            }
        }

        public override void OnApplyTemplate()
        {
            //this.Movement = Enums.XYMovement.TopToBottom;
            _htmlPanel = this.Content as WpfHtmlControl;
            _webBrowser = this.Header as WebBrowser;
            base.OnApplyTemplate();
        }

        private void Update(string s)
        {
            if (_updateLock == true)
                return;
            _updateLock = true;

            //_htmlEditor.Document.Blocks.Clear();
            //_htmlEditor.Document.Blocks.Add(new Paragraph(new Run(s)));
            Cursor = Cursors.Wait;

            try
            {
                //_htmlPanel.AvoidImagesLateLoading = !sample.FullName.Contains("Many images");
                _htmlPanel.Html = s;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to render HTML");
            }

            Cursor = Cursors.Arrow;
            _updateLock = false;

            UpdateWebBrowserHtml();
        }

        public void UpdateWebBrowserHtml()
        {
            if (_webBrowser.IsVisible == true)
            {
                //_webBrowser.NavigateToString(_useGeneratedHtml ? _htmlPanel.GetHtml() : GetFixedHtml());
                _webBrowser.NavigateToString(Html);
            }

            /// <summary>
            /// Fix the raw html by replacing bridge object properties calls with path to file with the data returned from the property.
            /// </summary>
            /// <returns>fixed html</returns>
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