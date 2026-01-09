using System;
using System.Collections.Generic;
using System.Text;
using NetPrints.Core;
using NetPrints.Core.Converters;
using NetPrintsEditor.Converters;

namespace NetPrints.WPF.Demo
{
    internal class SpecifierConverter : ISpecifierConverter
    {
        public string ConvertToIconPath(ISpecifier value)
        {
            if (value is MethodSpecifier methodSpecifier)
            {
                return OperatorUtil.IsOperator(methodSpecifier) ? "Operator_16x.png" : "Method_16x.png";

            }
            throw new Exception("Unsupported specifier type");
        }

        public string ConvertToText(ISpecifier value)
        {
            if (value is MethodSpecifier methodSpecifier)
            {
                return MethodSpecifierConverter.Convert(value);

            }
            throw new Exception("Unsupported specifier type");
        }
    }
}
