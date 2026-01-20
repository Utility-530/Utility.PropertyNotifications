using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WPF.ComboBoxes.Roslyn
{
    internal class SymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is IntelliSenseResult { Symbol.Item: ISymbol symbol } result)
            {
                if (parameter is string paramStr)
                {
                    if (paramStr == "Name")
                    {
                        return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    }
                    else if (paramStr == "Namespace" )
                    {
                        var _namespace = symbol.ContainingType?.ContainingNamespace.ToDisplayString()?? symbol.ContainingNamespace.ToDisplayString();
                        return _namespace;// typeSymbol?.ContainingNamespace.ToDisplayString() ?? "";
                    }
                    else
                        throw new Exception("Unknown parameter for TypeSpecifierConverter");
                }
            }
            else if(value is ISymbol _symbol)
            {
                if (parameter is string paramStr)
                {
                    if (paramStr == "Name")
                    {
                        return _symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    }
                    else if (paramStr == "Namespace")
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
