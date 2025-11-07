using System.Collections;
using System.Collections.Generic;

namespace Pather.CSharp.PathElements
{
    public abstract class Base : IPathElement
    {
        public IEnumerable Apply(CSharp.Selection target)
        {
            var results = new List<object>();
            foreach (var entriy in target.Entries)
            {
                results.Add(Apply(entriy));
            }
            var result = new CSharp.Selection(results);
            return result;
        }

        public abstract object Apply(object target);

        public abstract void Apply(object target, object value);
    }
}