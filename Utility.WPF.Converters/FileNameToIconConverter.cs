using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel.Design;
using Utility.WPF.Helper;
using System.Runtime.InteropServices;


namespace Utility.WPF.Converters
{
    /// <summary>
    /// <a href="https://github.com/markjulmar/mvvmhelpers"></a>
    /// This converter takes a filename and returns a valid ImageSource icon for it
    /// using the Windows Shell.
    /// </summary>
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class FilenameToIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.
        /// </param><param name="targetType">The type of the binding target property.
        /// </param><param name="parameter">The converter parameter to use.
        /// </param><param name="culture">The culture to use in the converter.
        /// </param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Don't hit the shell if in the designer.
            if (DesignModeHelper.IsInDesignMode)
                return DependencyProperty.UnsetValue;

            // Check input
            if (value == null || value.GetType() != typeof(string))
                return DependencyProperty.UnsetValue;

            try
            {
                return Win32.LoadIcon(value.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.
        /// </param><param name="targetType">The type to convert to.
        /// </param><param name="parameter">The converter parameter to use.
        /// </param><param name="culture">The culture to use in the converter.
        /// </param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }


        [StructLayout(LayoutKind.Sequential)]
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // Large icon
            public const uint SHGFI_SMALLICON = 0x1; // Small icon

            [DllImport("shell32.dll")]
            static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("User32.dll", SetLastError = true)]
            static extern bool DestroyIcon(IntPtr handle);

            public static ImageSource LoadIcon(string name)
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                SHGetFileInfo(name, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
                ImageSource imageSource;
                try
                {
                    imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(shinfo.hIcon, Int32Rect.Empty, null);
                }
                finally
                {
                    DestroyIcon(shinfo.hIcon);
                }
                return imageSource;
            }
        }
    }
}
