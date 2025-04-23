using System;
using System.Collections.Generic;
using System.Linq;
using Pather.CSharp.PathElements;

namespace Pather.CSharp
{
    public interface IPathElementSplitter
    {
        IList<IPathElement> ToPathElements(string path);
    }

    public class PathElementSplitter : IPathElementSplitter
    {
        private IList<IPathElementFactory> pathElementFactories;
        /// <summary>
        /// contains the path element factories used to resolve given paths
        /// more specific factories must be before more generic ones, because the first applicable one is taken
        /// </summary>
        public IList<IPathElementFactory> PathElementFactories
        {
            get { return pathElementFactories; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("The PathElementFactories must not be null");
                pathElementFactories = value;
            }
        }



        public IList<IPathElement> ToPathElements(string path)
        {
            var pathElements = new List<IPathElement>();
            var tempPath = path;
            while (tempPath.Length > 0)
            {
                var pathElement = createPathElement(tempPath, out tempPath);
                pathElements.Add(pathElement);
                //remove the dots chaining properties 
                //no PathElement could do this reliably
                //the only appropriate one would be Property, but there doesn't have to be a dot at the beginning (if it is the first PathElement, e.g. "Property1.Property2")
                //and I don't want that knowledge in PropertyFactory
                if (tempPath.StartsWith("."))
                    tempPath = tempPath.Remove(0, 1);
            }
            return pathElements;
            IPathElement createPathElement(string _path, out string newPath)
            {
                //get the first applicable path element type
                var pathElementFactory = PathElementFactories.Where(f => f.IsApplicable(_path)).FirstOrDefault();

                if (pathElementFactory == null)
                    throw new InvalidOperationException($"There is no applicable path element factory for {_path}");

                IPathElement result = pathElementFactory.Create(_path, out newPath);
                return result;
            }
        }
    }
}
