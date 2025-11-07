using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pather.CSharp.PathElements;

namespace Pather.CSharp
{
    public static class ResolverHelper
    {
        public static object ResolveSafe(this IResolver resolver, object target, string path)
        {
            try
            {
                return resolver.Get(target, path);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public static object Resolve(this IPathElementSplitter pathSplitter, object target, string path)
        {
            return resolve(pathSplitter.ToPathElements(path));

            object resolve(IList<IPathElement> pathElements)
            {
                var tempResult = target;
                foreach (var pathElement in pathElements)
                {
                    if (tempResult is Selection s)
                        tempResult = pathElement.Apply(s);
                    else
                        tempResult = pathElement.Apply(tempResult);
                }

                if (tempResult is Selection selection)
                    return selection.AsEnumerable();
                else
                    return tempResult;
            }
        }

        public static void Modify(this IPathElementSplitter pathSplitter, object target, string path, object value)
        {
            resolve(pathSplitter.ToPathElements(path));

            void resolve(IList<IPathElement> pathElements)
            {
                var tempResult = target;
                List<object> tempResults = new List<object>();
                tempResults.Add(tempResult);
                for (int i = 0; i < pathElements.Count - 1; i++)
                {
                    var pathElement = pathElements[i];
                    if (tempResult is Selection s)
                        tempResult = pathElement.Apply(s);
                    else
                        tempResult = pathElement.Apply(tempResult);
                    tempResults.Add(tempResult);
                }

                pathElements.LastOrDefault()?.Apply(tempResult, value);
                if (tempResult.GetType().GetTypeInfo().IsValueType)
                {
                    pathElements[pathElements.Count - 2].Apply(tempResults[tempResults.Count - 2], tempResult);
                }
            }
        }
    }
}