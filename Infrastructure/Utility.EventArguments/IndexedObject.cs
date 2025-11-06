namespace Utility.EventArguments
{
    public class IndexedObject
    {
        public IndexedObject(object @object, int index, int oldIndex)
        {
            Object = @object;
            Index = index;
            OldIndex = oldIndex;
        }

        public int Index { get; set; }
        public int OldIndex { get; }
        public object Object { get; }
    }
}