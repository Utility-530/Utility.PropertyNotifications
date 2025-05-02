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
using Microsoft.Xaml.Behaviors.Core;

namespace Leepfrog.WpfFramework
{
    public class CompareExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a behaviour to add a condition to a trigger
        /// </summary>
        /// <remarks>
        /// </remarks>

        public CompareExtension()
        {

        }

        private ComparisonCondition _comparison;

        public CompareExtension(object leftOperand, ComparisonConditionType op, object rightOperand)
        {
            _comparison = new ComparisonCondition();
            SetValueAndPreserveBinding(_comparison, ComparisonCondition.LeftOperandProperty, leftOperand);
            SetValueAndPreserveBinding(_comparison, ComparisonCondition.OperatorProperty, op);
            SetValueAndPreserveBinding(_comparison, ComparisonCondition.RightOperandProperty, rightOperand);
        }

        private static void SetValueAndPreserveBinding(DependencyObject obj, DependencyProperty prop, object value)
        {
            if ( value is BindingBase )
            {
                BindingOperations.SetBinding(obj, prop, value as BindingBase);
            }
            else
            {
                obj.SetValue(prop, value);
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if ( serviceProvider == null )
            {
                return null;
            }
            return _comparison;
        }
    }
}
