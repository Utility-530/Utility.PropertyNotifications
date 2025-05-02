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

namespace Leepfrog.WpfFramework
{
    public class AnyExtension :
            MarkupExtension
    {
        /// <summary>
        /// Markup extension that returns an array of objects
        /// </summary>
        /// <remarks>
        /// </remarks>

        private object[] _items;

        public AnyExtension()
        {

        }
        public AnyExtension(params object[] items)
        {
            _items = items;
        }
        public AnyExtension(object item1, object item2)
        {
            _items = new object[] { item1, item2 };
        }

        public AnyExtension(object item1, object item2, object item3)
        {
            _items = new object[] { item1, item2, item3 };
        }

        public AnyExtension(object item1, object item2, object item3, object item4)
        {
            _items = new object[] { item1, item2, item3, item4 };
        }
        public AnyExtension(object item1, object item2, object item3, object item4, object item5)
        {
            _items = new object[] { item1, item2, item3, item4, item5 };
        }
        public AnyExtension(object item1, object item2, object item3, object item4, object item5, object item6)
        {
            _items = new object[] { item1, item2, item3, item4, item5, item6 };
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _items;
        }

    }
}
