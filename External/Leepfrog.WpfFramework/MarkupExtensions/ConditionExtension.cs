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
using Microsoft.Xaml.Behaviors;
using System.Reflection;
using System.Xaml;

namespace Leepfrog.WpfFramework
{
    public class ConditionExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns a behaviour to add a condition to a trigger
        /// </summary>
        /// <remarks>
        /// </remarks>

        public ConditionExtension()
        {

        }

        private ConditionBehavior _behavior;

        public ConditionExtension(ForwardChaining chain, object comp1, object comp2)
        {
            if ( !( comp1 is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression() { ForwardChaining = chain };
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp1);
            condition.Conditions.Add((ComparisonCondition)comp2);
        }
        public ConditionExtension(ForwardChaining chain, object comp1, object comp2, object comp3)
        {
            if ( !( comp1 is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression() { ForwardChaining = chain };
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp1);
            condition.Conditions.Add((ComparisonCondition)comp2);
            condition.Conditions.Add((ComparisonCondition)comp3);
        }
        public ConditionExtension(ForwardChaining chain, object comp1, object comp2, object comp3, object comp4)
        {
            if ( !( comp1 is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression() { ForwardChaining = chain };
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp1);
            condition.Conditions.Add((ComparisonCondition)comp2);
            condition.Conditions.Add((ComparisonCondition)comp3);
            condition.Conditions.Add((ComparisonCondition)comp4);
        }
        public ConditionExtension(ForwardChaining chain, object comp1, object comp2, object comp3, object comp4, object comp5)
        {
            if ( !( comp1 is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression() { ForwardChaining = chain };
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp1);
            condition.Conditions.Add((ComparisonCondition)comp2);
            condition.Conditions.Add((ComparisonCondition)comp3);
            condition.Conditions.Add((ComparisonCondition)comp4);
            condition.Conditions.Add((ComparisonCondition)comp5);
        }
        public ConditionExtension(ForwardChaining chain, object comp1, object comp2, object comp3, object comp4, object comp5, object comp6)
        {
            if ( !( comp1 is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression() { ForwardChaining = chain };
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp1);
            condition.Conditions.Add((ComparisonCondition)comp2);
            condition.Conditions.Add((ComparisonCondition)comp3);
            condition.Conditions.Add((ComparisonCondition)comp4);
            condition.Conditions.Add((ComparisonCondition)comp5);
            condition.Conditions.Add((ComparisonCondition)comp6);
        }
        public ConditionExtension(ForwardChaining chain, object comp1, object comp2, object comp3, object comp4, object comp5, object comp6, object comp7)
        {
            if ( !( comp1 is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression() { ForwardChaining = chain };
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp1);
            condition.Conditions.Add((ComparisonCondition)comp2);
            condition.Conditions.Add((ComparisonCondition)comp3);
            condition.Conditions.Add((ComparisonCondition)comp4);
            condition.Conditions.Add((ComparisonCondition)comp5);
            condition.Conditions.Add((ComparisonCondition)comp6);
            condition.Conditions.Add((ComparisonCondition)comp7);
        }

        public ConditionExtension(ForwardChaining chain, object comp1, object comp2, object comp3, object comp4, object comp5, object comp6, object comp7, object comp8)
        {
            if ( !( comp1 is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression() { ForwardChaining = chain };
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp1);
            condition.Conditions.Add((ComparisonCondition)comp2);
            condition.Conditions.Add((ComparisonCondition)comp3);
            condition.Conditions.Add((ComparisonCondition)comp4);
            condition.Conditions.Add((ComparisonCondition)comp5);
            condition.Conditions.Add((ComparisonCondition)comp6);
            condition.Conditions.Add((ComparisonCondition)comp7);
            condition.Conditions.Add((ComparisonCondition)comp8);
        }
        public ConditionExtension(ForwardChaining chain, object comp1, object comp2, object comp3, object comp4, object comp5, object comp6, object comp7, object comp8, object comp9)
        {
            if ( !( comp1 is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression() { ForwardChaining = chain };
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp1);
            condition.Conditions.Add((ComparisonCondition)comp2);
            condition.Conditions.Add((ComparisonCondition)comp3);
            condition.Conditions.Add((ComparisonCondition)comp4);
            condition.Conditions.Add((ComparisonCondition)comp5);
            condition.Conditions.Add((ComparisonCondition)comp6);
            condition.Conditions.Add((ComparisonCondition)comp7);
            condition.Conditions.Add((ComparisonCondition)comp8);
            condition.Conditions.Add((ComparisonCondition)comp9);
        }

        public ConditionExtension(object leftOperand, ComparisonConditionType op, object rightOperand)
        {
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression();
            var comp = new ComparisonCondition();
            SetValueAndPreserveBinding(comp, ComparisonCondition.LeftOperandProperty, leftOperand);
            SetValueAndPreserveBinding(comp, ComparisonCondition.OperatorProperty, op);
            SetValueAndPreserveBinding(comp, ComparisonCondition.RightOperandProperty, rightOperand);
            condition.Conditions.Add(comp);
            _behavior.Condition = condition;
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

        public ConditionExtension(object comp)
        {
            if ( !( comp is ComparisonCondition ) )
            {
                return;
            }
            _behavior = new ConditionBehavior();
            var condition = new ConditionalExpression();
            _behavior.Condition = condition;
            condition.Conditions.Add((ComparisonCondition)comp);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _behavior;
        }
    }

    public class If
    {

        #region If
        // ********************************************************************
        public static ConditionBehavior GetIf(DependencyObject obj) => (ConditionBehavior)obj.GetValue(IfProperty); //was conditionbehav
        public static void SetIf(DependencyObject obj, ConditionBehavior value)
        {
            obj.SetValue(IfProperty, value);
        }
        public static readonly DependencyProperty IfProperty =
            DependencyProperty.RegisterAttached("If", typeof(ConditionBehavior), typeof(If), new PropertyMetadata(default(ConditionBehavior), If_Changed, If_CooerceValue));

        private static object If_CooerceValue(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return null;
            }
            var behaviors = Interaction.GetBehaviors(d);
            if (behaviors == null)
            {
                return null;
            }
            if (baseValue is ConditionBehavior condition)
            {
                behaviors.Add(condition);
            }
            return null;
        }

        private static void If_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        // ********************************************************************
        #endregion

    }
}
