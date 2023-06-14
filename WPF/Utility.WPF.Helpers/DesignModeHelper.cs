using System.ComponentModel;

namespace Utility.WPF.Helpers
{
    public class DesignModeHelper
    {
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// http://blog.galasoft.ch/posts/2009/09/detecting-design-time-mode-in-wpf-and-silverlight/
        /// Laurent Bugnion (GalaSoft)
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode
                        = (bool)DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(System.Windows.FrameworkElement))
                        .Metadata.DefaultValue;
                }

                return _isInDesignMode.Value;
            }
        }
    }
}