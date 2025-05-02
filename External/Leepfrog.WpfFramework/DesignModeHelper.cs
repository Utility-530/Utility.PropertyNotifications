using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Leepfrog.WpfFramework
{
    public static class DesignModeHelper
    {
        private static bool _isLoaded = false;
        private static bool _isInDesignMode = false;

        public static bool IsInDesignMode
        {
            get
            {
                if (!_isLoaded)
                {
                    var maybeExpressionUseLayoutRounding =
                        Application.Current.Resources["ExpressionUseLayoutRounding"] as bool?;
                    _isInDesignMode = maybeExpressionUseLayoutRounding ?? false; ;
                    _isLoaded = true;
                }
                return _isInDesignMode;
            }
        }
    }
}
