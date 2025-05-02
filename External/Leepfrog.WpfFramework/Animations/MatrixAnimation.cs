using Leepfrog.WpfFramework.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Leepfrog.WpfFramework.Animations
{
    public class MatrixAnimation : MatrixAnimationBase
    {
        public Matrix? From
        {
            set { SetValue(FromProperty, value); }
            get { return (Matrix?)GetValue(FromProperty); }
        }

        public static DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(Matrix?), typeof(MatrixAnimation),
                new PropertyMetadata(null));

        public Matrix? To
        {
            set { SetValue(ToProperty, value); }
            get { return (Matrix?)GetValue(ToProperty); }
        }

        public static DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(Matrix?), typeof(MatrixAnimation),
                new PropertyMetadata(null));

        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(MatrixAnimation),
                new UIPropertyMetadata(null));

        public MatrixAnimation()
        {
        }

        public MatrixAnimation(Matrix toValue, Duration duration)
        {
            To = toValue;
            Duration = duration;
        }

        public MatrixAnimation(Matrix toValue, Duration duration, FillBehavior fillBehavior)
        {
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        public MatrixAnimation(Matrix fromValue, Matrix toValue, Duration duration)
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
        }

        public MatrixAnimation(Matrix fromValue, Matrix toValue, Duration duration, FillBehavior fillBehavior)
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MatrixAnimation();
        }

        private bool _isInitialised = false;
        private double _fromScaleX;
        private double _fromScaleY;
        private double _fromAngle;
        private double _toScaleX;
        private double _toScaleY;
        private double _toAngle;

        protected override Matrix GetCurrentValueCore(Matrix defaultOriginValue, Matrix defaultDestinationValue, AnimationClock animationClock)
        {
            try
            {
                if (animationClock.CurrentProgress == null)
                {
                    return Matrix.Identity;
                }

                var normalizedTime = animationClock.CurrentProgress.Value;
                if (EasingFunction != null)
                {
                    normalizedTime = EasingFunction.Ease(normalizedTime);
                }

                var from = From ?? defaultOriginValue;
                var to = To ?? defaultDestinationValue;

                if ((from == null) || (to == null))
                {
                    return Matrix.Identity;
                }

                if (!_isInitialised)
                {
                    _fromScaleX = from.ExtractScaleX();
                    _fromScaleY = from.ExtractScaleY();
                    _fromAngle = from.ExtractAngle();
                    _toScaleX = to.ExtractScaleX();
                    _toScaleY = to.ExtractScaleY();
                    _toAngle = to.ExtractAngle();
                    if (_toAngle > (_fromAngle + 180))
                    {
                        _toAngle -= 360;
                    }
                    if (_toAngle < (_fromAngle - 180))
                    {
                        _toAngle += 360;
                    }
                    _isInitialised = true;
                }
                //normalizedTime = 1;
                var changeScaleX = (_toScaleX - _fromScaleX) * normalizedTime;
                var changeScaleY = (_toScaleY - _fromScaleY) * normalizedTime;
                var changeAngle = (_toAngle - _fromAngle) * normalizedTime;
                var changeOffsetX = ((to.OffsetX - from.OffsetX) * normalizedTime);
                var changeOffsetY = ((to.OffsetY - from.OffsetY) * normalizedTime);

                var newMatrix = Matrix.Identity;
                newMatrix.Rotate(_fromAngle + changeAngle);
                newMatrix.Scale(_fromScaleX + changeScaleX, _fromScaleY + changeScaleY);
                newMatrix.Translate(from.OffsetX + changeOffsetX, from.OffsetY + changeOffsetY);

                var toInv = to;
                toInv.Invert();

                var translatedTo = toInv.Transform(new Point(0, 0));
                var fromInv = from;
                fromInv.Invert();
                var translatedFrom = fromInv.Transform(new Point(0, 0));

                var newMatrix2 = from;
                var newScaleX = (_fromScaleX + changeScaleX) / _fromScaleX;
                var newScaleY = (_fromScaleY + changeScaleY) / _fromScaleY;
                var matrixCentrePoint = Matrix.Identity;
                matrixCentrePoint.Rotate(changeAngle);

                newMatrix2.Rotate(changeAngle);//,-(to.OffsetX - from.OffsetX)*_toScaleX/_fromScaleX,-(to.OffsetY-from.OffsetY )*_toScaleY/_fromScaleY );
                newMatrix2.Scale(newScaleX, newScaleY);//, (to.OffsetX - from.OffsetX), (to.OffsetY - from.OffsetY));
                newMatrix2.Translate(changeOffsetX, changeOffsetY);

                newMatrix2 = Matrix.Identity;
                newMatrix2.Translate(-translatedTo.X, -translatedTo.Y);
                newMatrix2.Rotate(_fromAngle + changeAngle);
                //newMatrix2.Scale(_fromScaleX , _fromScaleY );
                newMatrix2.Scale(_fromScaleX + changeScaleX, _fromScaleY + changeScaleY);

                var invMat2 = newMatrix2;
                invMat2.Invert();
                var translatedNowX = translatedTo.X + (translatedFrom.X - translatedTo.X) * (1 - normalizedTime);
                var translatedNowY = translatedTo.Y + (translatedFrom.Y - translatedTo.Y) * (1 - normalizedTime);
                var translatedNow = new Point(translatedNowX, translatedNowY);
                var transFrom = newMatrix2.Transform(translatedTo) - newMatrix2.Transform(translatedNow);
                newMatrix2.Translate(transFrom.X, transFrom.Y);
                //newMatrix2.TranslatePrepend(transFrom.X*(1-normalizedTime), transFrom.Y*1-(normalizedTime));

                //this.AddLog("MATRIXANI",$"from {from.OffsetX:0.00},{from.OffsetY:0.00} to {_toScaleX:0.00},{_toScaleY:0.00},{_toAngle:0.00} @ {normalizedTime:0.00} = {scaleX:0.00},{scaleY:0.00},{angle:0.00}");
                return newMatrix2;
            }
            catch (Exception ex)
            {
                this.AddLog(ex);
            }
            return Matrix.Identity;
        }
    }
}
