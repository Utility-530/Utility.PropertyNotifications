using System.Collections.Generic;
using System.Linq;
using Utility.Helpers.NonGeneric;

namespace UtilityWpf.Controls
{
    public class StructDataGrid : DynamicDataGrid
    {
        public StructDataGrid()
        {
        }

        protected override void ChangeItemsSource(System.Collections.IEnumerable value)
        {
            var t = value.First();
            var type = t.GetType();
            var props = type.GetProperties();
            //var fields = type.GetFields();

            var value2 = value.Cast<object>().Select(_ => props.Select(__ => new[] { new KeyValuePair<string, string>(__.Name, __.GetValue(_).ToString()) }));
            //.Concat(fields.Select(__ => new[] { new KeyValuePair<string, string>(__.Name, __.GetValue(_).ToString()) })));
            //new KeyValuePair<string, string>(
            ItemsSourceSubject.OnNext(value2);
        }
    }
}