using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Leepfrog.WpfFramework
{
    public class PowerEaseExtension : MarkupExtension
    {
        public PowerEaseExtension()
        {

        }

        private EasingFunctionBase _function;

        public PowerEaseExtension(EasingMode mode)
        {
            _function = new PowerEase() { EasingMode = mode };
        }

        public PowerEaseExtension(string mode)
        {
            _function = new PowerEase() { EasingMode = (EasingMode)Enum.Parse(typeof(EasingMode), mode) };
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _function;
        }
    }
}
