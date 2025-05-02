using Leepfrog.WpfFramework.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Leepfrog.WpfFramework.Behaviors
{
    public class AnimatableBinding
    {

        #region Binding

        public static object GetBinding(DependencyObject obj)
        {
            if (obj == null)
            {
                string.Empty.AddLog("GETBINDING", "obj is null");
                return null;
            }
            return (object)obj.GetValue(BindingProperty);
        }

        public static void SetBinding(DependencyObject obj, object value)
        {
            obj.SetValue(BindingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.RegisterAttached("Binding", typeof(object), typeof(AnimatableBinding), new PropertyMetadata(null,binding_Changed));

        private static void binding_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == null)
            {
                SetValue(d,GetBinding(d));
            }
        }
        #endregion

        #region Value

        public static object GetValue(DependencyObject obj)
        {
            if (obj == null)
            {
                string.Empty.AddLog("GETVALUE", "obj is null");
                return null;
            }
            return (object)obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, object value)
        {
            obj.SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(object), typeof(AnimatableBinding), new PropertyMetadata(null));

        #endregion

        #region GoCommand
        // ********************************************************************
        public static ICommand GoCommand => _goCommand;
        private static readonly ICommand _goCommand = new RelayCommand((param) => go(param as DependencyObject));

        // ********************************************************************
        #endregion

        #region BeforeChange Storyboard

        public static Storyboard GetBeforeChangeStoryboard(DependencyObject obj) => (Storyboard)obj.GetValue(BeforeChangeStoryboardProperty);

        public static void SetBeforeChangeStoryboard(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(BeforeChangeStoryboardProperty, value);
        }

        public static readonly DependencyProperty BeforeChangeStoryboardProperty =
            DependencyProperty.RegisterAttached("BeforeChangeStoryboard", typeof(Storyboard), typeof(AnimatableBinding), new PropertyMetadata(null));


        #endregion

        #region AfterChange Storyboard

        public static Storyboard GetAfterChangeStoryboard(DependencyObject obj) => (Storyboard)obj.GetValue(AfterChangeStoryboardProperty);

        public static void SetAfterChangeStoryboard(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(AfterChangeStoryboardProperty, value);
        }

        public static readonly DependencyProperty AfterChangeStoryboardProperty =
            DependencyProperty.RegisterAttached("AfterChangeStoryboard", typeof(Storyboard), typeof(AnimatableBinding), new PropertyMetadata(null));


        #endregion

        #region BeforeFinal Storyboard

        public static Storyboard GetBeforeFinalStoryboard(DependencyObject obj) => (Storyboard)obj.GetValue(BeforeFinalStoryboardProperty);

        public static void SetBeforeFinalStoryboard(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(BeforeFinalStoryboardProperty, value);
        }

        public static readonly DependencyProperty BeforeFinalStoryboardProperty =
            DependencyProperty.RegisterAttached("BeforeFinalStoryboard", typeof(Storyboard), typeof(AnimatableBinding), new PropertyMetadata(null));

        #endregion

        #region AfterFinal Storyboard

        public static Storyboard GetAfterFinalStoryboard(DependencyObject obj) => (Storyboard)obj.GetValue(AfterFinalStoryboardProperty);

        public static void SetAfterFinalStoryboard(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(AfterFinalStoryboardProperty, value);
        }

        public static readonly DependencyProperty AfterFinalStoryboardProperty =
            DependencyProperty.RegisterAttached("AfterFinalStoryboard", typeof(Storyboard), typeof(AnimatableBinding), new PropertyMetadata(null));

        #endregion

        #region By
        // ********************************************************************
        public static double GetBy(DependencyObject obj) => (double)obj.GetValue(ByProperty);
        public static void SetBy(DependencyObject obj, double value)
        {
            obj.SetValue(ByProperty, value);
        }
        public static readonly DependencyProperty ByProperty =
            DependencyProperty.RegisterAttached("By", typeof(double), typeof(AnimatableBinding), new PropertyMetadata(default(double)));
        // ********************************************************************
        #endregion

        #region Duration
        // ********************************************************************
        public static TimeSpan GetDuration(DependencyObject obj) => (TimeSpan)obj.GetValue(DurationProperty);
        public static void SetDuration(DependencyObject obj, TimeSpan value)
        {
            obj.SetValue(DurationProperty, value);
        }
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.RegisterAttached("Duration", typeof(TimeSpan), typeof(AnimatableBinding), new PropertyMetadata(default(TimeSpan)));
        // ********************************************************************
        #endregion


        #region EasingFunction
        // ********************************************************************
        public static EasingFunctionBase GetEasingFunction(DependencyObject obj) => (EasingFunctionBase)obj.GetValue(EasingFunctionProperty);
        public static void SetEasingFunction(DependencyObject obj, EasingFunctionBase value)
        {
            obj.SetValue(EasingFunctionProperty, value);
        }
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.RegisterAttached("EasingFunction", typeof(EasingFunctionBase), typeof(AnimatableBinding), new PropertyMetadata(default(EasingFunctionBase)));
        // ********************************************************************
        #endregion


        private static void startStoryboardThen(Storyboard beforeStoryboard, Action then, Storyboard afterStoryboard)
        {
            if (beforeStoryboard == null)
            {
                then();
                startStoryboard(afterStoryboard);
            }
            else
            {
                EventHandler completed = null;
                completed = (s, e) =>
                {
                    beforeStoryboard.Completed -= completed;
                    then();
                    startStoryboard(afterStoryboard);
                };
                beforeStoryboard.Completed += completed;
                beforeStoryboard.Begin();
            }
        }
        private static void startStoryboard(Storyboard storyboard)
        {
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }
        private static void go(DependencyObject target)
        {
            //-----------------------------------------------------------------
            var oldValue = GetValue(target);
            var newValue = GetBinding(target);
            var doubleConverter = TypeDescriptor.GetConverter(typeof(double));
            Updater updater = null;
            //-----------------------------------------------------------------
            if (oldValue == null)
            {
                doubleConverter.AddLog("ANIMATABLEBINDING", "Old Value is NULL");
                oldValue = 0;
            }
            if (newValue == null)
            {
                doubleConverter.AddLog("ANIMATABLEBINDING", "New Value is NULL");
                newValue = 0;
            }
            //-----------------------------------------------------------------
            double oldStringValue = 0;
            if (oldValue is string)
            {
                double.TryParse((string)oldValue, out oldStringValue);
                oldValue = oldStringValue;
            }
            double newStringValue = 0;
            if (newValue is string)
            {
                double.TryParse((string)newValue, out oldStringValue);
                newValue = newStringValue;
            }
            //-----------------------------------------------------------------
            // IF OLD VALUE AND NEW VALUE ARE BOTH NUMBERS...
            Type[] numericTypes = { typeof(double), typeof(float), typeof(Int32), typeof(Int16), typeof(Byte), typeof(SByte), typeof(UInt32), typeof(UInt16), typeof(Int64), typeof(UInt64) };
            if (
                ((numericTypes.Contains(oldValue.GetType())) || (doubleConverter.CanConvertFrom(oldValue.GetType())))
             && ((numericTypes.Contains(newValue.GetType())) || (doubleConverter.CanConvertFrom(newValue.GetType())))
                )
            {
                //-----------------------------------------------------------------
                // LET'S GET THE NUMBERS
                var oldDouble = getDouble(oldValue);
                var newDouble = getDouble(newValue);
                //-----------------------------------------------------------------
                updater = new Updater(target, oldDouble, newDouble);
                return;
                //-----------------------------------------------------------------
            }
            //-----------------------------------------------------------------
            // NOT NUMERIC, SO JUST START FINAL STORYBOARD
            startStoryboardThen(GetBeforeFinalStoryboard(target), () => updateValue(target, newValue), GetAfterFinalStoryboard(target));
            //-----------------------------------------------------------------
        }
        
        private static double getDouble(object o)
        {
            IConvertible convert = o as IConvertible;
            if (convert != null)
            {
                return convert.ToDouble(null);
            }
            else
            {
                return 0;
            }
        }


        private static void updateValue(DependencyObject target, object newValue)
        {
            // UPDATE VALUE
            target.SetValue(ValueProperty, newValue);
        }


        private class Updater
        {
            private DependencyObject _target;
            private DateTimeOffset _startTime;
            private TimeSpan _duration;
            private double _by;
            private double _from;
            private double _to;
            private double _currentValue;
            private EasingFunctionBase _easingFunction;
            private Storyboard _beforeChangeStoryboard;
            private Storyboard _afterChangeStoryboard;
            private Storyboard _beforeFinalStoryboard;
            private Storyboard _afterFinalStoryboard;
            private bool _isRunningBeforeAnimation = false;

            public Updater(DependencyObject target, double from, double to)
            {
                //-----------------------------------------------------------------
                if (target == null)
                {
                    this.AddLog("UPDATER", "TARGET IS NULL");
                    return;
                }
                //-----------------------------------------------------------------
                _target = target;
                _startTime = DateTimeOffset.Now;
                _from = from;
                _currentValue = from;
                _to = to;
                _duration = GetDuration(target);
                _by = GetBy(target);
                _easingFunction = GetEasingFunction(target);
                _beforeChangeStoryboard = GetBeforeChangeStoryboard(target);
                _afterChangeStoryboard = GetAfterChangeStoryboard(target);
                _beforeFinalStoryboard = GetBeforeFinalStoryboard(target);
                _afterFinalStoryboard = GetAfterFinalStoryboard(target);
                //-----------------------------------------------------------------
                var diff = to - from;
                if (Math.Sign(_by) != Math.Sign(diff))
                {
                    _by = -_by;
                }
                diff = Math.Abs(diff);
                //-----------------------------------------------------------------
                // IF DURATION IS ZERO
                // OR DIFFERENCE IS LESS THAN STEP
                if (
                    (_duration == null) 
                 || (_duration == TimeSpan.Zero)
                 || (diff <= Math.Abs(_by))
                   )
                {
                    //-----------------------------------------------------------------
                    // JUMP STRAIGHT TO FINAL VALUE
                    startFinalAnimation();
                    //-----------------------------------------------------------------
                    return;
                    //-----------------------------------------------------------------
                }
                //-----------------------------------------------------------------
                // DO FIRST STEP RIGHT AWAY
                _from += _by;
                //-----------------------------------------------------------------
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                //-----------------------------------------------------------------
            }

            private void startFinalAnimation()
            {
                //-----------------------------------------------------------------
                var beforeAni = _beforeFinalStoryboard ?? _beforeChangeStoryboard;
                var afterAni = _afterFinalStoryboard ?? _afterChangeStoryboard;
                //-----------------------------------------------------------------
                startStoryboardThen(
                    beforeAni,
                    () => SetValue(_target, _to),
                    afterAni
                    );
                //-----------------------------------------------------------------
            }

            private void startChangeAnimation()
            {
                //-----------------------------------------------------------------
                var newValue = _currentValue;
                _isRunningBeforeAnimation = true;
                startStoryboardThen(
                    _beforeChangeStoryboard,
                    () =>
                    {
                        SetValue(_target, newValue);
                        _isRunningBeforeAnimation = false;
                    },
                    _afterChangeStoryboard
                    );
                //-----------------------------------------------------------------
            }

            private void CompositionTarget_Rendering(object sender, EventArgs e)
            {
                //-----------------------------------------------------------------
                if (_isRunningBeforeAnimation)
                {
                    return;
                }
                //-----------------------------------------------------------------
                var easeTime = (DateTimeOffset.Now - _startTime).TotalMilliseconds / _duration.TotalMilliseconds;
                if (easeTime >= 1)
                {
                    // IT'S ALL OVER
                    // START FINAL ANIMATION
                    startFinalAnimation();
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                    return;
                }
                if (_easingFunction != null)
                {
                    easeTime = _easingFunction.Ease(easeTime);
                }
                //-----------------------------------------------------------------
                var newValue = _from + ((_to - _from) * easeTime);
                var changed = false;
                //-----------------------------------------------------------------
                while
                    (
                     ( (_by < 0) && (newValue <= (_currentValue + _by)) )
                  || ( (_by > 0) && (newValue >= (_currentValue + _by)) ) 
                    )
                {
                    _currentValue += _by;
                    changed = true;
                }
                //-----------------------------------------------------------------
                if (!changed)
                {
                    return;
                }
                //-----------------------------------------------------------------
                startChangeAnimation();
                //-----------------------------------------------------------------
            }
        }

    }
}
    