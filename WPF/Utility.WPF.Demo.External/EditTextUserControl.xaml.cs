using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.External
{
    /// <summary>
    /// Interaction logic for EditTextUserControl.xaml
    /// </summary>
    public partial class EditTextUserControl : UserControl
    {
        private AdornerLayer layer;

        public EditTextUserControl()
        {
            InitializeComponent();
            //            Observable.FromEventPattern<MouseButtonEventHandler, MouseEventArgs>(
            //                   s => editpath.PreviewMouseUp += s,
            //                   s => editpath.PreviewMouseUp -= s)
            //                .Subscribe(a =>
            //            {
            //                layer ??= AdornerLayer.GetAdornerLayer(TextBlock);
            //                var adorner = new DirectEditTextBoxAdorner(TextBlock, TextBlock);
            //                layer.Add(adorner);

            ////                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
            ////                    s => adorner.LostFocus += s,
            ////                    s => adorner.LostFocus -= s)
            ////                .Subscribe(a =>
            ////                {
            ////   layer ??= AdornerLayer.GetAdornerLayer(TextBlock);
            ////   layer.Clear();
            ////});

            //            });
        }
    }

    public abstract class DirectEditAdorner : Adorner
    {
        protected readonly VisualCollection visualChildren;
        protected FrameworkElement _mainUiElement;

        /// <summary>
        /// Creates a new instance of <see cref="DirectEditAdorner"/>.
        /// </summary>
        /// <param name="adornedElement">The UI element the adorner should attach to.</param>
        /// <param name="adornedTerminal">The terminal the adorner can edit the data of.</param>
        public DirectEditAdorner(UIElement adornedElement, UIElement adornedTerminal)
            : base(adornedElement)
        {
            AdornedTerminal = adornedTerminal;
            visualChildren = new VisualCollection(this);
        }

        /// <summary>
        /// The terminal this adorner can edit the data of.
        /// </summary>
        public UIElement AdornedTerminal { get; set; }

        /// <summary>
        /// Whether the type of the adorned terminal is editable with this class.
        /// </summary>
        //public abstract bool IsDirectlyEditableType { get; }

        /// <inheritdoc/>
        protected override int VisualChildrenCount => visualChildren.Count;

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_mainUiElement == null)
            {
                return Size.Empty;
            }

            double width = _mainUiElement.Width;
            double height = _mainUiElement.Height;

            double x = GetRelativeXBasedOnTerminalDirection(width);
            double y = GetRelativeYBasedOnTerminalDirection(height);
            _mainUiElement.Arrange(new Rect(0, 0, width, height));
            return finalSize;
        }

        /// <inheritdoc/>
        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }

        private double GetRelativeXBasedOnTerminalDirection(double width)
        {
            return width;
            //return TerminalAdornerHelpers.GetVisualXBasedOnTerminalDirection(width, direction);
        }

        private double GetRelativeYBasedOnTerminalDirection(double height)
        {
            return height;
            //var direction = AdornedTerminal.TerminalRotation;
            //return TerminalAdornerHelpers.GetVisualYBasedOnTerminalDirection(height, direction);
        }
    }

    /// <summary>
    /// An adorner that allows users to directly edit numeric and string type data on a <see cref="Terminal"/>.
    /// </summary>
    public class DirectEditTextBoxAdorner : DirectEditAdorner
    {
        private readonly TextBox textBox;

        /// <summary>
        /// Creates a new instance of <see cref="DirectEditTextBoxAdorner"/>.
        /// </summary>
        /// <param name="adornedElement">The UI element the adorner should attach to.</param>
        /// <param name="adornedTerminal">The terminal the adorner can edit the data of.</param>
        public DirectEditTextBoxAdorner(FrameworkElement adornedElement, UIElement adornedTerminal)
            : base(adornedElement, adornedTerminal)
        {
            if (adornedTerminal == null)
            {
                return;
            }

            textBox = new TextBox
            {
                IsHitTestVisible = true,
                Margin = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(16, 16, 16)),
                Foreground = new SolidColorBrush(Color.FromRgb(178, 178, 178)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(96, 96, 96)),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Width = adornedElement.ActualWidth,
                Height = adornedElement.ActualHeight,
            };
            textBox.Loaded += TextBoxLoaded;
            Binding binding = new Binding
            {
                Source = this,
                //Path = new PropertyPath(nameof(DirectEditTextBoxText)),
                Mode = BindingMode.OneWay,
            };
            textBox.SetBinding(TextBox.TextProperty, binding);
            textBox.KeyDown += KeyDownHandler;
            textBox.LostFocus += LostFocusHandler;
            textBox.LostKeyboardFocus += LostFocusHandler;
            textBox.AutoWordSelection = true;
            _mainUiElement = textBox;
            visualChildren.Add(textBox);
        }

        private void FocusTextBox()
        {
            textBox.Focus();
            Keyboard.Focus(textBox);
        }

        private void TextBoxLoaded(object sender, RoutedEventArgs e)
        {
            FocusTextBox();
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //AdornedTerminal.SetAdorner(null);
                var layer = AdornerLayer.GetAdornerLayer(AdornedElement);
                layer.Clear();
            }
        }

        private void LostFocusHandler(object sender, RoutedEventArgs e)
        {
            //AdornedTerminal.SetAdorner(null);
            var layer = AdornerLayer.GetAdornerLayer(AdornedElement);
            layer.Clear();
        }
    }
}