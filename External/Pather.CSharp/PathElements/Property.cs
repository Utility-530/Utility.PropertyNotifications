using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pather.CSharp.PathElements
{
    public class Property : Base
    {
        protected string property;

        public Property(string propertyName)
        {
            this.property = propertyName;
        }

        public override object Apply(object target)
        {
            return get(target)?.GetValue(target) ?? throw new ArgumentException($"The property {property} could not be found.");
        }

        public override void Apply(object target, object value)
        {
            (get(target) ?? throw new ArgumentException($"The property {property} could not be found.")).SetValue(target, value);
        }

        protected PropertyInfo get(object target)
        {
            return target.GetType().GetRuntimeProperty(property);
        }
    }
}
