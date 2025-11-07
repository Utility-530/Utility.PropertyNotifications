using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Pather.CSharp.PathElements
{
    public class Enumerable : Base
    {
        private int index;

        public Enumerable(int index)
        {
            this.index = index;
        }

        public override object Apply(object target)
        {
            //index lower than 0 doesn't have to be checked, because the IsApplicable check doesn't apply to negative values

            var enumerable = target as IEnumerable;
            if (enumerable != null)
            {
                var i = 0;
                foreach (var value in enumerable)
                {
                    if (i == index)
                        return value;
                    i++;
                }
                //if no value is returned by now, it means that the index is too high
                throw new IndexOutOfRangeException($"The index {index} is too high. Maximum index is {i - 1}.");
            }
            else
            {
                //if the object is no enumerable, it may have an indexer
                var indexProperties = target.GetType().GetRuntimeProperties().Where(p => p.GetIndexParameters().Length > 0);
                var appropriateIndexProperty = indexProperties.Where(p => p.GetIndexParameters().Length == 1 && p.GetIndexParameters()[0].ParameterType == typeof(int)).FirstOrDefault();
                if (appropriateIndexProperty == null) throw new ArgumentException("The target does not have an indexer that takes exactly 1 int parameter");
                return appropriateIndexProperty.GetValue(target, new object[] { index });
            }
        }

        public override void Apply(object target, object value)
        {
            if (target is IList list)
            {
                if (list.Count > index)
                {
                    list[index] = value;
                }
                else if (index == list.Count)
                {
                    list.Add(value);
                }
                else
                {
                    throw new Exception("2ds3");
                }
            }
            else
            {
                throw new Exception("sd");
            }
        }
    }
}