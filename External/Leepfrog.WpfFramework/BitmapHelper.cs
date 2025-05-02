namespace Leepfrog.WpfFramework.Helpers
{
    using System.ComponentModel;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Media.Imaging;
    using System.Drawing;
    using System.IO;

    public static class BitmapHelper
    {
        public static BitmapImage ConvertBitmapToImageSource(this Bitmap src)
        {
            var ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}