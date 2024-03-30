using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Windows.Forms;
using Svg;

using System.Windows.Interop;
using System.Windows.Shapes;
using AdonisUI.Controls;
using MaterialDesignThemes.Wpf;

namespace Utility.WPF.Demo.SandBox
{
    //public class IconAdorner : Adorner
    //{
    //    public IconAdorner(UIElement adornedElement) : base(adornedElement)
    //    {
    //    }


    //    //string icon = "M24 8.77h-2.468v7.565h-1.425V8.77h-2.462V7.53H24zm-6.852 7.565h-4.821V7.53h4.63v1.24h-3.205v2.494h2.953v1.234h-2.953v2.604h3.396zm-6.708 0H8.882L4.78 9.863a2.896 2.896 0 0 1-.258-.51h-.036c.032.189.048.592.048 1.21v5.772H3.157V7.53h1.659l3.965 6.32c.167.261.275.442.323.54h.024c-.04-.233-.06-.629-.06-1.185V7.529h1.372zm-8.703-.693a.868.829 0 0 1-.869.829.868.829 0 0 1-.868-.83.868.829 0 0 1 .868-.828.868.829 0 0 1 .869.829Z";

    //    string svg = "<svg role=\"img\" viewBox=\"0 0 24 24\" xmlns=\"http://www.w3.org/2000/svg\"><title>.NET</title><path d=\"M24 8.77h-2.468v7.565h-1.425V8.77h-2.462V7.53H24zm-6.852 7.565h-4.821V7.53h4.63v1.24h-3.205v2.494h2.953v1.234h-2.953v2.604h3.396zm-6.708 0H8.882L4.78 9.863a2.896 2.896 0 0 1-.258-.51h-.036c.032.189.048.592.048 1.21v5.772H3.157V7.53h1.659l3.965 6.32c.167.261.275.442.323.54h.024c-.04-.233-.06-.629-.06-1.185V7.529h1.372zm-8.703-.693a.868.829 0 0 1-.869.829.868.829 0 0 1-.868-.83.868.829 0 0 1 .868-.828.868.829 0 0 1 .869.829Z\"/></svg>";
    //    protected override void OnRender(DrawingContext drawingContext)
    //    {
    //        string svg = this.svg;
    //        string text = "some text";
    //        //BitmapImage myBitmapImage = new BitmapImage(new Uri(@"Images\PomoDomo.ico", UriKind.RelativeOrAbsolute));



    //        var myBitmapImage = Bitmap2BitmapImage(SvgTextToBitmap(svg));
    //        // Draw image
    //        drawingContext.DrawImage(myBitmapImage, new Rect(0, 0, myBitmapImage.Width*3, myBitmapImage.Height*3));

    //        //var typeFace = new Typeface(new System.Windows.Media.FontFamily("Verdana"), FontStyles.Normal,
    //        //    FontWeights.ExtraBold, FontStretches.UltraCondensed);
    //        //var formatedText = new FormattedText(text,
    //        //      CultureInfo.InvariantCulture,
    //        //      System.Windows.FlowDirection.LeftToRight,
    //        //      typeFace,
    //        //      10,
    //        //      System.Windows.Media.Brushes.Black);

    //        ////Center the text on Image
    //        //int pointY = (int)(myBitmapImage.Height - formatedText.Height) / 2;
    //        //int pointX = (int)(myBitmapImage.Width - formatedText.Width) / 2;

    //        //drawingContext.DrawText(formatedText, new System.Windows.Point(pointX, pointY));

    //    }

    //    private static System.Drawing.Bitmap SvgTextToBitmap(string text)
    //    {

    //        byte[] bytes = Encoding.UTF8.GetBytes(text);
    //        using (var stream = new MemoryStream(bytes))
    //        {
    //            var svgDoc = SvgDocument.Open<SvgDocument>(stream, null);
    //            return svgDoc.Draw();
    //        }

    //    }

    //    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    //    public static extern bool DeleteObject(IntPtr hObject);

    //    private static BitmapSource Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
    //    {
    //        IntPtr hBitmap = bitmap.GetHbitmap();
    //        BitmapSource retval;

    //        try
    //        {
    //            retval = (BitmapSource)Imaging.CreateBitmapSourceFromHBitmap(
    //                         hBitmap,
    //                         IntPtr.Zero,
    //                         Int32Rect.Empty,
    //                         BitmapSizeOptions.FromEmptyOptions());
    //        }
    //        finally
    //        {
    //            DeleteObject(hBitmap);
    //        }

    //        return retval;
    //    }
    //}

    //public class IconAdorner : Utility.WPF.Adorners.FrameworkElementAdorner
    //{
    //    public IconAdorner(FrameworkElement adornedElement) : base(adornedElement)
    //    {

    //        this.Adorners = new Adorners.AdornerCollection(adornedElement);
    //        this.Adorners.Add(new PackIcon { 
    //            Width = 24,
    //            Height=24, 
    //            Kind = PackIconKind.Abacus,
    //            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
    //            VerticalAlignment = VerticalAlignment.Center,
    //            Foreground = Brushes.Black
    //        });

    //         //< materialDesign:PackIcon
    //         //                   Width = "42"
    //         //                   Height = "42"
    //         //                   HorizontalAlignment = "Center"
    //         //                   VerticalAlignment = "Center"
    //         //                   Kind = "{Binding Path=Kind, RelativeSource={RelativeSource TemplatedParent}, Mode=OneWay, TargetNullValue={x:Static materialDesign:PackIconKind.Error}}" />
    //    }
    //}
}
