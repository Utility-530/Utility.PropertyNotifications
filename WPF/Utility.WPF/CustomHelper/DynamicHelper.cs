using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Reflection;

namespace CustomHelper
{
    public static class DynmamicHelper
    {
        public static List<Dynamic> OnGetData(IEnumerable enumerable, string key, string value)
        {
            var keys = ((IEnumerable)enumerable.First()).GetPropertyRefValues<object>(key);

            try
            {
                foreach (var k in keys)
                    Dynamic.AddProperty((string)k, typeof(string));
            }
            catch (System.Exception ex)
            {
            }
            var data = new List<Dynamic>();

            foreach (var en in enumerable)
            {
                var values = ((IEnumerable)en).GetPropertyRefValues<object>(value);
                Dynamic customer1 = new Dynamic();// { FirstName = "Julie", LastName = "Smith" };
                foreach (var val in values.Cast<object>().Zip(keys.Cast<object>(), (a, b) => new { a, b }))
                    customer1.SetPropertyValue((string)val.b, val.a);

                data.Add(customer1);
            }

            return data;
        }
    }
}