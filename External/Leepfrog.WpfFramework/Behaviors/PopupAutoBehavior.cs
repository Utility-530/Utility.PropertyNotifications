using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Leepfrog.WpfFramework.Behaviors
{
    public class PopupAutoBehavior
    {

        /// <summary>
        /// Sets a popup control to stay open until click/touch elsewhere
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PopupAutoBehavior), new PropertyMetadata(false,isEnabled_PropertyChanged));

        

        private static void isEnabled_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //-----------------------------------------------------------------
            if (!(d is Popup popup))
            {
                return;
            }
            if (DesignerProperties.GetIsInDesignMode(popup))
            {
                return;
            }
            //-----------------------------------------------------------------
            popup.StaysOpen = false;
            popup.Opened += popup_Opened;
            popup.Closed += popup_Closed;
            //-----------------------------------------------------------------
        }

        private static void popup_Closed(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------
            var popup = sender as Popup;
            if (PopupActive == popup)
            {
                PopupActive = null;
            }
            //-----------------------------------------------------------------
        }

        private static void popup_Opened(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------
            var popup = sender as Popup;
            PopupActive = popup;
            //-----------------------------------------------------------------
        }

        public static event EventHandler PopupActiveChanged;
        private static Popup _popupActive = null;
        public static Popup PopupActive { get => _popupActive; set { _popupActive = value; PopupActiveChanged?.Invoke(null, EventArgs.Empty); } }

    }
}
