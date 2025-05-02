using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Leepfrog.WpfFramework
{
    public class RotateTransformExtension : MarkupExtension
    {
        private RotateTransform _trans;

        public RotateTransformExtension()
        {
            _trans = new RotateTransform();
        }

        public RotateTransformExtension(double degrees) : this()
        {
            _trans.Angle = degrees;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _trans;
        }
    }
    public class ScaleTransformExtension : MarkupExtension
    {
        private ScaleTransform _trans;

        public ScaleTransformExtension()
        {
            _trans = new ScaleTransform();
        }

        public ScaleTransformExtension(double scaleX, double scaleY) : this()
        {
            _trans.ScaleX = scaleX;
            _trans.ScaleY = scaleY;
        }
        public ScaleTransformExtension(double scale) : this()
        {
            _trans.ScaleX = scale;
            _trans.ScaleY = scale;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _trans;
        }
    }
}
