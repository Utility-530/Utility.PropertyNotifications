using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Utility.WPF.Controls.External
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/28879606/how-to-exactly-center-rendered-text-inside-a-border/29020629#29020629">
    /// </summary>
    public partial class CenteredText : ScrollViewer
    {
        public CenteredText()
        {
            InitializeComponent();
          
        }

        public static readonly DependencyProperty ElementProperty = DependencyProperty
            .Register("Element", typeof(FrameworkElement), typeof(CenteredText),
            new PropertyMetadata(OnElementChanged));

        private static void OnElementChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var elem = e.NewValue as FrameworkElement;
            var ct = d as CenteredText;
            if (elem != null)
            {
                elem.Loaded += ct.Content_Loaded;
                ct.Content = elem;
            }
        }

        public FrameworkElement Element
        {
            get { return (FrameworkElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        void Content_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement elem = sender as FrameworkElement;
            int w = (int)elem.ActualWidth;
            int h = (int)elem.ActualHeight;
            var rtb = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(elem);

            Rect rc;
            if (TryFindGlyphs(rtb, out rc))
            {
                if (rc.Height > this.ActualHeight || rc.Width > this.ActualWidth)
                {
                    return; // todo: error handling
                }
                double desiredV = rc.Top - 0.5 * (this.ActualHeight - rc.Height);
                double desiredH = rc.Left - 0.5 * (this.ActualWidth - rc.Width);

                if (desiredV > 0)
                {
                    this.ScrollToVerticalOffset(desiredV);
                }
                else
                {
                    elem.Margin = new Thickness(elem.Margin.Left, elem.Margin.Top - desiredV,
                        elem.Margin.Right, elem.Margin.Bottom);
                }
                if (desiredH > 0)
                {
                    this.ScrollToHorizontalOffset(desiredH);
                }
                else
                {
                    elem.Margin = new Thickness(elem.Margin.Left - desiredH, elem.Margin.Top,
                        elem.Margin.Right, elem.Margin.Bottom);
                }
            }
        }

        bool TryFindGlyphs(BitmapSource src, out Rect rc)
        {
            int left = int.MaxValue;
            int toRight = -1;
            int top = int.MaxValue;
            int toBottom = -1;

            int w = src.PixelWidth;
            int h = src.PixelHeight;
            uint[] buf = new uint[w * h];
            src.CopyPixels(buf, w * sizeof(uint), 0);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // background is assumed to be fully transparent, i.e. 0x00000000 in Pbgra
                    if (buf[x + y * w] != 0)
                    {
                        if (x < left) left = x;
                        if (x > toRight) toRight = x;
                        if (y < top) top = y;
                        if (y > toBottom) toBottom = y;
                    }
                }
            }

            rc = new Rect(left, top, toRight - left, toBottom - top);
            return (toRight > left) && (toBottom > top);
        }
    }

}
