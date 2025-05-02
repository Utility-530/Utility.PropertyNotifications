using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Leepfrog.WpfFramework.Converters;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace Leepfrog.WpfFramework
{
    public class IfListExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a multibinding to do an IF then else
        /// </summary>
        /// <remarks>
        /// </remarks>



        public IfListExtension()
        {

        }

        private IfListConverter.AnyAll _type;
        private object _items;
        private PropertyPath _path;
        private object _in2;
        private object _returnIfTrue;
        private object _returnIfFalse;

        public IfListExtension(IfListConverter.AnyAll type, object items, object in1, object in2, object returnIfTrue, object returnIfFalse)
        {
            _type = type;
            _path = (in1 as Binding)?.Path;
            _items = items;
            _in2 = in2;
            _returnIfTrue = returnIfTrue;
            _returnIfFalse = returnIfFalse;
        }

        private BindingBase convertBinding(object param, IValueConverter conv = null)
        {
            Binding binding;
            if (param == null)
            {
                return new Binding("(NULL)");
            }
            if (param is Binding)
            {
                binding = (param as Binding);
            }
            else
            {
                binding = new Binding();
                binding.Source = param;
            }
            if (
                (conv != null)
             && (binding.Converter == null)
               )
            {
                binding.Converter = conv;
            }
            return binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // MULTIBINDING DOESN'T AUTOMATICALLY UPDATE WHEN THE COLLECTION CHANGES...
            // SO WE NEED TO MANUALLY HOOK UP THE COLLECTION CHANGE EVENT FOR ITEMS
            // AND ALSO HOOK UP A BINDING TO THE CHOSEN PATH ON ALL THE EXISTING ITEMS! YUK!
            // WHEN ANY ONE OF THEM CHANGES, WE NEED TO CALL BINDINGEXPRESSION.INVALIDATETARGET
            // TODO: MOVE THIS CODE TO A SUBCLASS OF MULTIBINDING - IT SHOULDN'T BE IN THE MARKUP EXTENSION!

            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var targetObject = (target.TargetObject as DependencyObject);
            var targetProperty = (target.TargetProperty as DependencyProperty);

            if (target.TargetObject == null)
            {
                return this;
            }

            if (target.TargetObject.GetType().FullName == "System.Windows.SharedDp")
            {
                this.AddLog("IFLIST", $"providing to SharedOp");
                return this;
            }

            //this.AddLog("IFLIST", $"providing to target '{ targetObject }'/'{target.TargetObject}'.'{ targetProperty?.Name }'");

            var multiBinding = new IfListMultiBinding();
            multiBinding.Mode = BindingMode.OneWay;
            multiBinding.Converter = new IfListConverter(_type, _path);
            multiBinding._path = _path;
            multiBinding.recordTarget(targetObject, targetProperty);
            multiBinding.Bindings.Add(convertBinding(_in2));
            multiBinding.Bindings.Add(convertBinding(_returnIfTrue));
            multiBinding.Bindings.Add(convertBinding(_returnIfFalse));
            multiBinding.Bindings.Add(convertBinding(multiBinding));
            multiBinding.Bindings.Add(convertBinding(_items));
            return multiBinding.ProvideValue(serviceProvider);

        }

    }

    public static class Logger
    {
        public static void AddLog(this object instance, string message) { }
        public static void AddLog(this object instance, string key, string message) { }
        public static void AddLog(this object instance, Exception message) { }

    }
}
