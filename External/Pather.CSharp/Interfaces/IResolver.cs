namespace Pather.CSharp
{
    public interface IResolver
    {
        /// <summary>
        /// Returns the object defined by the path.
        /// Any access exception (e.g. NullReference) is propagated.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        object Get(object target, string path);
    }
}