using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Utility.PropertyNotifications;

namespace Utility.WPF.ComboBoxes.Roslyn.Demo
{
    public enum CommonTypes
    {
        Boolean, Integer, Double, String, DateTime, All = Boolean | Integer | Double | String | DateTime
    }

    public static class CommonTypesHelper
    {
        extension(CommonTypes type)
        {
            public Type? ToType()
            {
                switch (type)
                {
                    case CommonTypes.Integer:
                        return typeof(int);
                    case CommonTypes.Boolean:
                        return typeof(bool);
                    case CommonTypes.String:
                        return typeof(string);
                    case CommonTypes.Double:
                        return typeof(double);
                    case CommonTypes.DateTime:
                        return typeof(DateTime);
                    case CommonTypes.All:
                        return null;
                }
                throw new ArgumentOutOfRangeException();
            }
            public SpecialType? ToSpecialType()
            {
                switch (type)
                {
                    case CommonTypes.Integer:
                        return SpecialType.System_Int32;
                    case CommonTypes.Boolean:
                        return SpecialType.System_Boolean;
                    case CommonTypes.String:
                        return SpecialType.System_String;
                    case CommonTypes.Double:
                        return SpecialType.System_Double;
                    case CommonTypes.DateTime:
                        return SpecialType.System_DateTime;
                    case CommonTypes.All:
                        return null;
                }
                throw new ArgumentOutOfRangeException();
            }
        
        }
    }
}
