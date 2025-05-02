using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Leepfrog.WpfFramework.ExtensionMethods
{
    public static class Extensions
    {
        public static Uri ToUri(this string path)
        {
            return new Uri($@"file://{path.Replace(@"\", @"/")}");
        }
    }
    public static class MatrixExtensions
    {
        public static double ExtractScaleX(this Matrix mat)
        {
            return Math.Sqrt((Math.Pow(mat.M11, 2) + Math.Pow(mat.M12, 2)));
        }
        public static double ExtractScaleY(this Matrix mat)
        {
            return Math.Sqrt((Math.Pow(mat.M21, 2) + Math.Pow(mat.M22, 2)));
        }
        public static double ExtractAngle(this Matrix mat)
        {
            return (Math.Atan2(mat.M12, mat.M11) * 180) / Math.PI;
        }
        public static string ToString2(this Matrix mat)
        {
            return $"{mat.M11:0.00},{mat.M12:0.00};{mat.M21:0.00},{mat.M22:0.00};{mat.OffsetX:0.00};{mat.OffsetY:0.00}";
        }
    }


}
