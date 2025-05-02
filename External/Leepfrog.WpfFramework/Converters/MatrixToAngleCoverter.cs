using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// extracts the angle component of a matrix
    /// </summary>
    public class MatrixToAngleConverter : IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if (value is Matrix matrix)
            {
                //-----------------------------------------------------------------
                var radians = Math.Atan2(matrix.M21, matrix.M11);
                var degrees = radians * 180 / Math.PI;
                return degrees;
                //-----------------------------------------------------------------
            }
            else
            {
                //-----------------------------------------------------------------
                return Binding.DoNothing;
                //-----------------------------------------------------------------
            }
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            throw new NotImplementedException();
            //=================================================================
        }

        //****************************************************************************
        #endregion
    }
}
