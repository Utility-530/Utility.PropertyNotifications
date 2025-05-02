using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Leepfrog.WpfFramework.Controls
{
    public class DialSlider : Slider
    {
        private static PropertyMetadata _metadataOriginal;
        static DialSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialSlider), new FrameworkPropertyMetadata(typeof(DialSlider)));
            _metadataOriginal = Slider.ValueProperty.GetMetadata(typeof(DialSlider));
            Slider.ValueProperty.OverrideMetadata(typeof(DialSlider), new FrameworkPropertyMetadata(_metadataOriginal.DefaultValue, value_changed, value_coerce));
        }

        private double _touchAngle = 0.0;

        private double _valueExact;
        private bool _isCaptured;

        private double ValueExact
        {
            get { return _valueExact; }
            set { _valueExact = value;
                var coerced = (double)value_coerce(this, _valueExact); if (Value != coerced) { Value = coerced; } }
        }

        public DialSlider()
        {
            this.PreviewTouchDown += ControlKnob_PreviewTouchDown;
            this.PreviewTouchMove += ControlKnob_PreviewTouchMove;
            this.PreviewTouchUp += ControlKnob_PreviewTouchUp;
            this.PreviewMouseLeftButtonDown += ControlKnob_PreviewMouseLeftButtonDown;
            this.PreviewMouseMove += ControlKnob_PreviewMouseMove;
            this.PreviewMouseLeftButtonUp += ControlKnob_PreviewMouseLeftButtonUp;
        }

        static private void value_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _metadataOriginal.PropertyChangedCallback(d, e);
        }

        static private object value_coerce(DependencyObject d, object baseValue)
        {
            var that = d as DialSlider;
            var coercedValue = (double)_metadataOriginal.CoerceValueCallback(d, baseValue);
            var coercedValueOrig = coercedValue;
            var rounded = (int)Math.Round(that._valueExact);
            if (rounded != (int)Math.Round(coercedValue))
            {
                that._valueExact = coercedValue;
            }
            return coercedValue;
        }

        private void ControlKnob_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //=================================================================
            _isCaptured = false;
            this.ReleaseMouseCapture();
            //=================================================================
        }

        private void ControlKnob_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //=================================================================
            if (_isCaptured)
            {
                var pos = e.GetPosition(this);
                processMove(pos);
            }
            //=================================================================
        }

        private void ControlKnob_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //=================================================================
            this.CaptureMouse();
            var pos = e.GetPosition(this);
            // get angle from centre of control
            getStartAngle(pos);
            _isCaptured = true;
            //=================================================================
        }

        private void getStartAngle(Point pos)
        {
            //=================================================================
            var angleAndDistance = calculateAngle(pos);
            if (angleAndDistance == null)
            {
                _touchAngle = 0;
            }
            else
            {
                _touchAngle = angleAndDistance.Value.Angle;
            }
            //=================================================================
        }

        private void ControlKnob_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            //=================================================================
            this.ReleaseTouchCapture(e.TouchDevice);
            //=================================================================
        }

        private void ControlKnob_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //=================================================================
            var pos = e.GetTouchPoint(this).Position;
            // get angle from centre of control
            getStartAngle(pos);
            this.CaptureTouch(e.TouchDevice);
            //=================================================================
        }

        private void ControlKnob_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            //=================================================================
            var pos = e.GetTouchPoint(this).Position;
            processMove(pos);
            //=================================================================
        }

        private struct AngleAndDistance
        {
            public double Angle;
            public double Distance;
        }
        private AngleAndDistance? calculateAngle(Point pos)
        {
            //=================================================================
            var vector = new Vector(
                pos.X - (this.RenderSize.Width / 2),
                pos.Y - (this.RenderSize.Height / 2)
                );
            //-----------------------------------------------------------------
            if ((vector.X == 0) && (vector.Y == 0))
            {
                // we are dead centre...  don't do anything
                return null;
            }
            //-----------------------------------------------------------------
            return new AngleAndDistance()
            {
                Angle = Math.Atan2(vector.Y, vector.X) * 180 / Math.PI,
                Distance = vector.Length
            };
            //=================================================================
        }


        private void processMove(Point pos)
        {
            //=================================================================
            // get angle from centre of control
            var angleAndDistance = calculateAngle(pos);
            //-----------------------------------------------------------------
            if (angleAndDistance == null)
            {
                // can't do anything if we're dead centre!
                return;
            }
            //-----------------------------------------------------------------
            var newAngle = angleAndDistance.Value.Angle;
            var dist = angleAndDistance.Value.Distance;
            //-----------------------------------------------------------------
            // get change in angle
            var deltaAngle = newAngle - _touchAngle;
            if (deltaAngle > 180)
            {
                // eg 359 becomes -1
                // eg 181 becomes -179
                deltaAngle = deltaAngle - 360;
            }
            else if (deltaAngle < -180)
            {
                // eg -359 becomes +1
                // eg -181 becomes +179
                deltaAngle = deltaAngle + 360;
            }
            //-----------------------------------------------------------------
            // adjust value, based on angle and distance from centre
            // ( further away = finer adjustment )
            // eg dist = 10 => multiply factor of 2
            // eg dist = 100 => multiply factor of 0.2
            // eg dist = 1000 => multiply factor of 0.02
            var multiplyFactor = 2.0;
            if (dist < 5)
            {
                multiplyFactor = 0;
            }
            if (dist > 10)
            {
                multiplyFactor = 20.0 / dist;
            }
            var deltaValue = deltaAngle * multiplyFactor;
            ValueExact += deltaValue;
            //-----------------------------------------------------------------
            //Angle += deltaAngle;
            _touchAngle = newAngle;
            //=================================================================
        }
    }
}
