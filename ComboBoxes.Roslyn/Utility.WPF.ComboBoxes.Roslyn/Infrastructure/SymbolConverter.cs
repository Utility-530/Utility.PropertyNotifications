using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Utility.PatternMatchings;
using Utility.Roslyn;

namespace Utility.WPF.ComboBoxes.Roslyn
{
    public class SymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is Result { } result)
            {
                if (result is { Symbol.Item: ISymbol symbol })
                {
                    if (parameter is string paramStr)
                    {
                        if (paramStr == "Selected")
                        {
                            return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                        }
                        else if (paramStr == "Alternate")
                        {
                            var _namespace = symbol.ContainingType?.ContainingNamespace.ToDisplayString() ?? symbol.ContainingNamespace.ToDisplayString();
                            return _namespace;// typeSymbol?.ContainingNamespace.ToDisplayString() ?? "";
                        }
                        else
                            throw new Exception("Unknown parameter for TypeSpecifierConverter");
                    }
                }
                else if (result is { Symbol.Item: PropertyInfo propertyInfo })
                {
                    if (parameter is string _paramStr)
                    {
                        if (_paramStr == "Selected")
                        {
                           
                            return null;// propertyInfo.ToString();
                        }
                        else if (_paramStr == "Alternate")
                        {
                            var _namespace = propertyInfo?.PropertyType.Name.ToString();
                            return _namespace;// typeSymbol?.ContainingNamespace.ToDisplayString() ?? "";
                        }
                        else
                            throw new Exception("Unknown parameter for TypeSpecifierConverter");
                    }
                }
            }
            else if (value is ISymbol _symbol)
            {
                if (parameter is string paramStr)
                {
                    if (paramStr == "Name")
                    {
                        return _symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    }
                    else if (paramStr == "Alternate")
                    {
                        var _namespace = _symbol.ContainingType?.ContainingNamespace.ToDisplayString() ?? _symbol.ContainingNamespace.ToDisplayString();
                        return _namespace;// typeSymbol?.ContainingNamespace.ToDisplayString() ?? "";
                    }
                    else
                        throw new Exception("Unknown parameter for TypeSpecifierConverter");
                }
            }

            return DependencyProperty.UnsetValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
