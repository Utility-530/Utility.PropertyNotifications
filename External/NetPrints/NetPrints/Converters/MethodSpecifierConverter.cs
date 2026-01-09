using NetPrints.Core;
using System;
using System.Globalization;
using System.Reflection;

namespace NetPrints.Core.Converters
{
    public class MethodSpecifierConverter 
    {
        public static string Convert(object value)
        {
            if (value is IMethodSpecifier methodSpecifier)
            {
                string name;

                // Check if the method is an operator
                if (OperatorUtil.TryGetOperatorInfo(methodSpecifier, out OperatorInfo operatorInfo))
                {
                    name = $"Operator {operatorInfo.DisplayName}";
                }
                else
                {
                    name = methodSpecifier.Name;
                }

                string paramTypeNames = string.Join(", ", methodSpecifier.Parameters);
                string s = $"{methodSpecifier.DeclaringType} {name} ({paramTypeNames})";

                if (methodSpecifier.ReturnTypes.Count > 0)
                {
                    s += $" : {string.Join(", ", methodSpecifier.ReturnTypes)}";
                }

                return s;
            }

            return value.ToString();
        }


    }
}
