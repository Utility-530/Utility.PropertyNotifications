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
using System.ComponentModel;

namespace Leepfrog.WpfFramework.Converters
{
    public class MathConverter :
            MarkupExtension,
          IMultiValueConverter,
          IValueConverter
    {
        /// <summary>
        /// Value converter that performs arithmetic calculations over its argument(s)
        /// </summary>
        /// <remarks>
        /// MathConverter can act as a value converter, or as a multivalue converter (WPF only).
        /// It is also a markup extension (WPF only) which allows to avoid declaring resources,
        /// ConverterParameter must contain an arithmetic expression over converter arguments. Operations supported are +, -, * and /
        /// Single argument of a value converter may referred as {0}
        /// Arguments of multi value converter may be referred as {0}, {1}, {2}, {3}, {4}, ...
        /// The converter supports arithmetic expressions of arbitrary complexity, including nested subexpressions
        /// </remarks>

        private Dictionary<string, IExpression> _storedExpressions = new Dictionary<string, IExpression>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new object[] { value }, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public string StringFormat { get; set; }
        public IValueConverter Converter { get; internal set; }
        public object ConverterParameter { get; internal set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // wrapper to allow us to convert the final return value!
            var targetTypeInternal = targetType;
            
            // if there is a converter, use the correct type
            if (Converter!=null)
            {
                var attributes = Converter.GetType().GetCustomAttributes(
                    typeof(ValueConversionAttribute), false);

                if (attributes.Length == 1)
                {
                    targetTypeInternal = (attributes[0] as ValueConversionAttribute).SourceType;
                }
            }
            var ret = ConvertInternal(values, targetTypeInternal, parameter, culture);
            if (
                (Converter == null)
             || (ret == DependencyProperty.UnsetValue)
               )
            {
                return ret;
            }
            return Converter.Convert(ret, targetType, ConverterParameter, culture);
        }
        public object ConvertInternal(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // WORKAROUNDS FOR BINDTO EXTENSION AT DESIGN TIME!
                if ( DesignerProperties.GetIsInDesignMode(new DependencyObject()) )
                {
                    var newValues = new List<object>();
                    foreach (var val in values)
                    {
                        var newVal = val;
                        if ( newVal is MarkupExtension )
                        {
                            newVal = ( newVal as MarkupExtension ).ProvideValue(new Target());
                        }
                        if ( newVal is BindingBase )
                        {
                            newVal = EvalBinding(newVal as BindingBase);
                        }
                        newValues.Add(newVal);
                    }
                    values = newValues.ToArray();
                }
                double result = (double)Parse(parameter.ToString()).Eval(values);
                if (targetType == typeof(CornerRadius)) return new CornerRadius(result);
                if (targetType == typeof(CornerRadius?)) return new CornerRadius(result);
                if (targetType == typeof(decimal)) return (decimal)result;
                if (targetType == typeof(decimal?)) return (decimal?)result;
                if (targetType == typeof(string) ) return StringFormat == null ? result.ToString() : result.ToString(StringFormat);
                if (targetType == typeof(int)) return (int)result;
                if (targetType == typeof(int?)) return (int?)result;
                if (targetType == typeof(short)) return (short)result;
                if (targetType == typeof(short?)) return (short?)result;
                if (targetType == typeof(double)) return result;
                if (targetType == typeof(double?)) return result;
                if (targetType == typeof(long)) return (long)result;
                if (targetType == typeof(long?)) return (long?)result;
                if (targetType == typeof(GridLength)) return new GridLength(result);
                if (targetType == typeof(GridLength?)) return new GridLength(result);
                if (targetType == typeof(bool)) return (result != 0);
                if (targetType == typeof(bool?)) return (result != 0);
                if (targetType == typeof(Visibility)) return (result != 0) ? Visibility.Visible : Visibility.Collapsed;
                if (targetType == typeof(object)) return result;
                throw new ArgumentException(String.Format("Unsupported target type {0}", targetType.FullName));
                 
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        protected virtual void ProcessException(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        private IExpression Parse(string s)
        {
            IExpression result = null;
            if (!_storedExpressions.TryGetValue(s, out result))
            {
                result = new Parser().Parse(s);
                _storedExpressions[s] = result;
            }

            return result;
        }

        interface IExpression
        {
            decimal Eval(object[] args);
        }

        class Constant : IExpression
        {
            private decimal _value;

            public Constant(string text)
            {
                if (!decimal.TryParse(text, out _value))
                {
                    throw new ArgumentException(String.Format("'{0}' is not a valid number", text));
                }
            }

            public decimal Eval(object[] args)
            {
                return _value;
            }
        }

        class Variable : IExpression
        {
            private int _index;

            public Variable(string text)
            {
                if (!int.TryParse(text, out _index) || _index < 0)
                {
                    throw new ArgumentException(String.Format("'{0}' is not a valid parameter index", text));
                }
            }

            public Variable(int n)
            {
                _index = n;
            }

            public decimal Eval(object[] args)
            {
                if (_index >= args.Length)
                {
                    throw new ArgumentException(String.Format("MathConverter: parameter index {0} is out of range. {1} parameter(s) supplied", _index, args.Length));
                }
                try
                {
                    return System.Convert.ToDecimal(args[_index]);
                }
                catch
                {
                    // consider this mod to accept any string as a 1 for boolean operations
                    //try
                    //{
                    //    return string.IsNullOrWhiteSpace(System.Convert.ToString(args[_index])) ? 0m : 1m;
                    //}
                    //catch
                    //{
                        return 0m;// throw;
                    //}
                }
            }
        }

        class BinaryOperation : IExpression
        {
            private Func<decimal, decimal, decimal> _operation;
            private IExpression _left;
            private IExpression _right;

            public BinaryOperation(char operation, IExpression left, IExpression right)
            {
                _left = left;
                _right = right;
                switch (operation)
                {
                    case '+': _operation = (a, b) => (a + b); break;
                    case '-': _operation = (a, b) => (a - b); break;
                    case '*': _operation = (a, b) => (a * b); break;
                    case '/': _operation = (a, b) => (a / b); break;
                    case '÷': _operation = (a, b) => Math.Floor (a / b); break;
                    case '%': _operation = (a, b) => (a % b); break;
                    case '@': _operation = (a, b) => ((a != 0) && (b != 0)) ? 1 : 0; break;
                    case '|': _operation = (a, b) => ((a != 0) || (b != 0)) ? 1 : 0; break;
                    default: throw new ArgumentException("Invalid operation " + operation);
                }
            }

            public decimal Eval(object[] args)
            {
                return _operation(_left.Eval(args), _right.Eval(args));
            }
        }

        class Negate : IExpression
        {
            private IExpression _param;

            public Negate(IExpression param)
            {
                _param = param;
            }

            public decimal Eval(object[] args)
            {
                return -_param.Eval(args);
            }
        }
        class Invert : IExpression
        {
            private IExpression _param;

            public Invert(IExpression param)
            {
                _param = param;
            }

            public decimal Eval(object[] args)
            {
                return _param.Eval(args) == 0 ? 1 : 0;
            }
        }

        class Parser
        {
            private string text;
            private int pos;

            public IExpression Parse(string text)
            {
                try
                {
                    pos = 0;
                    this.text = text;
                    var result = ParseExpression();
                    RequireEndOfText();
                    return result;
                }
                catch (Exception ex)
                {
                    string msg =
                        String.Format("MathConverter: error parsing expression '{0}'. {1} at position {2}", text, ex.Message, pos);

                    throw new ArgumentException(msg, ex);
                }
            }

            private IExpression ParseExpression()
            {
                IExpression left = ParseTerm();

                while (true)
                {
                    if (pos >= text.Length) return left;

                    var c = text[pos];

                    if (c == '+' || c == '-' || c=='@' || c=='|')
                    {
                        ++pos;
                        IExpression right = ParseTerm();
                        left = new BinaryOperation(c, left, right);
                    }
                    else
                    {
                        return left;
                    }
                }
            }

            private IExpression ParseTerm()
            {
                IExpression left = ParseFactor();

                while (true)
                {
                    if (pos >= text.Length) return left;

                    var c = text[pos];

                    if (c == '*' || c == '/' || c == '%' || c == '÷')
                    {
                        ++pos;
                        IExpression right = ParseFactor();
                        left = new BinaryOperation(c, left, right);
                    }
                    else
                    {
                        return left;
                    }
                }
            }

            private IExpression ParseFactor()
            {
                SkipWhiteSpace();
                if (pos >= text.Length) throw new ArgumentException("Unexpected end of text");

                var c = text[pos];

                if (c == '+')
                {
                    ++pos;
                    return ParseFactor();
                }

                if (c == '-')
                {
                    ++pos;
                    return new Negate(ParseFactor());
                }

                if ((c == '~')||(c == '!'))
                {
                    ++pos;
                    return new Invert(ParseFactor());
                }

                if (c == 'x' || c == 'a') return CreateVariable(0);
                if (c == 'y' || c == 'b') return CreateVariable(1);
                if (c == 'z' || c == 'c') return CreateVariable(2);
                if (c == 't' || c == 'd') return CreateVariable(3);
                if (c == 'u' || c == 'e') return CreateVariable(4);

                if (c == '(')
                {
                    ++pos;
                    var expression = ParseExpression();
                    SkipWhiteSpace();
                    Require(')');
                    SkipWhiteSpace();
                    return expression;
                }

                if (c == '{')
                {
                    ++pos;
                    var end = text.IndexOf('}', pos);
                    if (end < 0) { --pos; throw new ArgumentException("Unmatched '{'"); }
                    if (end == pos) { throw new ArgumentException("Missing parameter index after '{'"); }
                    var result = new Variable(text.Substring(pos, end - pos).Trim());
                    pos = end + 1;
                    SkipWhiteSpace();
                    return result;
                }

                const string decimalRegEx = @"(\d+\.?\d*|\d*\.?\d+)";
                var match = Regex.Match(text.Substring(pos), decimalRegEx);
                if (match.Success)
                {
                    pos += match.Length;
                    SkipWhiteSpace();
                    return new Constant(match.Value);
                }
                else
                {
                    throw new ArgumentException(String.Format("Unexpeted character '{0}'", c));
                }
            }

            private IExpression CreateVariable(int n)
            {
                ++pos;
                SkipWhiteSpace();
                return new Variable(n);
            }

            private void SkipWhiteSpace()
            {
                while (pos < text.Length && Char.IsWhiteSpace((text[pos]))) ++pos;
            }

            private void Require(char c)
            {
                if (pos >= text.Length || text[pos] != c)
                {
                    throw new ArgumentException("Expected '" + c + "'");
                }

                ++pos;
            }

            private void RequireEndOfText()
            {
                if (pos != text.Length)
                {
                    throw new ArgumentException("Unexpected character '" + text[pos] + "'");
                }
            }
        }
        public struct Target : IServiceProvider, IProvideValueTarget
        {
            private readonly DependencyObject _targetObject;
            private readonly DependencyProperty _targetProperty;

            public Target(DependencyObject targetObject, DependencyProperty targetProperty)
            {
                _targetObject = targetObject;
                _targetProperty = targetProperty;
            }

            public object GetService(Type serviceType)
            {
                if ( serviceType == typeof(IProvideValueTarget) )
                    return this;
                return null;
            }

            object IProvideValueTarget.TargetObject { get { return _targetObject; } }
            object IProvideValueTarget.TargetProperty { get { return _targetProperty; } }
        }
        class DummyDO : DependencyObject
        {
            public object Value
            {
                get { return (object)GetValue(ValueProperty); }
                set { SetValue(ValueProperty, value); }
            }

            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(DummyDO), new UIPropertyMetadata(null));

        }

        public object EvalBinding(BindingBase b)
        {
            // TODO: Put DummyDO into a global class
            // TODO: Make this a one-time only binding?
            DummyDO d = new DummyDO();
            BindingOperations.SetBinding(d, DummyDO.ValueProperty, b);
            var value = d.Value;
            BindingOperations.ClearBinding(d, DummyDO.ValueProperty);
            return value;
        }

    }
}
