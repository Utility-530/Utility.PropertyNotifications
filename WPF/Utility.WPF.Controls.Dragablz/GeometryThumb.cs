using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Utility.WPF.Controls.Dragablz
{
    /// <summary>
    /// A button thumb with geometry content
    /// </summary>
    public class GeometryThumb : Thumb, ICommandSource
    {
        public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register("Geometry", typeof(Geometry), typeof(GeometryThumb), new PropertyMetadata(null));
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(GeometryThumb), new PropertyMetadata(Brushes.WhiteSmoke));
        public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.Register("PressedBrush", typeof(Brush), typeof(GeometryThumb), new PropertyMetadata(Brushes.Gray));
        //   public static readonly DependencyProperty CommandProperty =  DependencyProperty.Register("Command", typeof(ICommand), typeof(GeometryThumb), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty CommandParameterProperty;
        public static readonly DependencyProperty CommandTargetProperty;

        static GeometryThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryThumb), new FrameworkPropertyMetadata(typeof(GeometryThumb)));

            CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(GeometryThumb));
            CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(GeometryThumb));
            CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(GeometryThumb));
        }

        public GeometryThumb()
        {
        }

        public Geometry Geometry
        {
            get { return (Geometry)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        public Brush PressedBrush
        {
            get { return (Brush)GetValue(PressedBrushProperty); }
            set { SetValue(PressedBrushProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Reflects the parameter to pass to the CommandProperty upon execution.
        /// </summary>
        [Bindable(true), Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        ///     The target element on which to fire the command.
        /// </summary>
        [Bindable(true), Category("Action")]
        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }
    }
}