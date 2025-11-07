using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TheArtOfDev.HtmlRenderer.WPF;
using Utility.WPF.Controls.Base;

namespace Utility.WPF.Controls.Html
{
    public interface IHtmlControl
    {
        string Html { get; set; }
    }

    public class HtmlControl : SplitControl, IHtmlControl
    {
        private bool _updateLock;
        private HtmlPanel? _htmlPanel;
        private System.Windows.Controls.WebBrowser _webBrowser;
        private bool applyFlag = false;
        private bool update = false;

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register("Html", typeof(string), typeof(HtmlControl), new PropertyMetadata(changed));

        static HtmlControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HtmlControl), new FrameworkPropertyMetadata(typeof(HtmlControl)));
        }

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HtmlControl view && e.NewValue is string s)
            {
                view.Update(s);
            }
        }

        public override void OnApplyTemplate()
        {
            //this.Movement = Enums.XYMovement.TopToBottom;
            _htmlPanel = this.Content as HtmlPanel;
            _webBrowser = this.Header as WebBrowser;
            applyFlag = true;
            if (update == true)
                Update(Html);
            base.OnApplyTemplate();
        }

        private void Update(string s)
        {
            update = true;
            if (applyFlag == false)
                return;

            if (_updateLock == true)
                return;
            _updateLock = true;

            //_htmlEditor.Document.Blocks.Clear();
            //_htmlEditor.Document.Blocks.Add(new Paragraph(new Run(s)));
            Cursor = Cursors.Wait;

            try
            {
                //_htmlPanel.AvoidImagesLateLoading = !sample.FullName.Contains("Many images");
                _htmlPanel.Text = s;
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
            if (_webBrowser.IsVisible)
            {
                //_webBrowser.NavigateToString(_useGeneratedHtml ? _htmlPanel.GetHtml() : GetFixedHtml());
                _webBrowser.NavigateToString(Html);
            }

            /// <summary>
            /// Fix the raw html by replacing bridge object properties calls with path to file with the data returned from the property.
            /// </summary>
            /// <returns>fixed html</returns>
        }
    }
}