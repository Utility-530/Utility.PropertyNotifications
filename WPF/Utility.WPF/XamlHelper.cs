using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;



namespace Leepfrog.WpfFramework
{
    public class XamlHelper
    {
        public static BindingBase ConvertBinding(object param, IValueConverter conv = null)
        {
            Binding ret;

            if (param is Binding binding)
            {
                ret = binding;
            }
            else
            {
                ret = new Binding
                {
                    Source = param
                };
            }

            if (ret.Converter == null && ret.Converter != conv)
            {
                try
                {
                    ret.Converter = conv;
                }
                catch
                {
                    throw new Exception($"XamlHelper can't set converter on Binding {ret.Path}");
                }
            }

            if (ret.Mode == BindingMode.TwoWay)               //|| (ret.Mode == BindingMode.Default)
            {
                try
                {
                    ret.Mode = BindingMode.OneWay;
                }
                catch
                {
                    throw new Exception($"XamlHelper can't set mode on Binding {ret.Path}");
                }
            }
            return ret;
        }

        public static object ProcessPossibleMarkupExtension(IServiceProvider serviceProvider, object param)
        {
            //=================================================================
            // WORKAROUNDS FOR BINDTO EXTENSION AT DESIGN TIME!
            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //{
            //-----------------------------------------------------------------
            if ((param is MarkupExtension markupExtension) && param is not Binding)
            {
                param = markupExtension.ProvideValue(serviceProvider);
            }

            if (param is BindingExpression bindingExp)
            {
                param = bindingExp.ParentBinding;
            }

            return param;
        }

        public static BindingBase ConvertPossibleMarkupExtensionToBinding(IServiceProvider serviceProvider, object param, IValueConverter conv = null)
        {
            return ConvertBinding(ProcessPossibleMarkupExtension(serviceProvider, param), conv);
        }

        public static bool IsTargetNull(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget target || target.TargetProperty == null;
        }

        public static bool IsTemplate(IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            // return true if null or SharedDP
            return target.TargetObject == null || target.TargetObject.GetType().FullName == "System.Windows.SharedDp";
        }
    }
}
