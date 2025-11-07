using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utility.WPF.Controls.SplitButtons
{
    [TemplatePart(Name = "PART_ToggleButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class PopupButton : HeaderedContentControl
    {
        private ToggleButton _toggleButton;
        private Popup _popup;

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PopupButton),
                new FrameworkPropertyMetadata(typeof(PopupButton)));
        }

        public PopupButton()
        {
            Loaded += OnLoaded;
        }

        // IsPopupOpen Dependency Property
        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(
                "IsPopupOpen",
                typeof(bool),
                typeof(PopupButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        // PopupPlacement Dependency Property
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register(
                "PopupPlacement",
                typeof(PlacementMode),
                typeof(PopupButton),
                new PropertyMetadata(PlacementMode.Top));

        public PlacementMode PopupPlacement
        {
            get { return (PlacementMode)GetValue(PopupPlacementProperty); }
            set { SetValue(PopupPlacementProperty, value); }
        }

        // StaysOpen Dependency Property
        public static readonly DependencyProperty StaysOpenProperty =
            DependencyProperty.Register(
                "StaysOpen",
                typeof(bool),
                typeof(PopupButton),
                new PropertyMetadata(false));

        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Unhook old events
            if (_toggleButton != null)
                _toggleButton.Checked -= OnToggleButtonChecked;

            if (_popup != null)
                _popup.Closed -= OnPopupClosed;

            // Get template parts
            _toggleButton = GetTemplateChild("PART_ToggleButton") as ToggleButton;
            _popup = GetTemplateChild("PART_Popup") as Popup;

            // Hook new events
            if (_toggleButton != null)
                _toggleButton.Checked += OnToggleButtonChecked;

            if (_popup != null)
                _popup.Closed += OnPopupClosed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Ensure popup closes when clicking elsewhere
            if (_popup != null && !StaysOpen)
            {
                _popup.StaysOpen = false;
            }
        }

        private void OnToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            IsPopupOpen = _toggleButton.IsChecked == true;
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (_toggleButton != null)
                _toggleButton.IsChecked = false;

            IsPopupOpen = false;
        }
    }
}