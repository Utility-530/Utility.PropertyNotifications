using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class FieldNameAttribute(string fieldName) : Attribute
    {
        public string FieldName { get; } = fieldName;
    }
}
