using System.Collections.ObjectModel;
using Views.Trees;

namespace Utility.Nodes.Demo
{
    internal class CustomFilter : ITreeViewFilter
    {
        public bool Filter(object item)
        {
            if (item is IReadOnlyTree { Data: IMethodDescriptor { Type: { } type } })
            {
                if (type.IsArray)
                {
                    return false;
                }
            }


            if (item is IReadOnlyTree { Data: IDescriptor { ParentType: { } componentType, Name: { } displayName } propertyNode })
            {
                if (componentType.Name == "Array")
                {
                    if (displayName == "IsFixedSize")
                        return false;
                    if (displayName == "IsReadOnly")
                        return false;
                    if (displayName == "IsSynchronized")
                        return false;
                    if (displayName == "LongLength")
                        return false;
                    if (displayName == "Length")
                        return false;
                    if (displayName == "Rank")
                        return false;
                    if (displayName == "SyncRoot")
                        return false;
                }
                if (componentType.Name == "String")
                {

                    if (displayName == "Length")
                        return false;
                }

                if (componentType.IsAssignableTo(typeof(IList)))
                {
                    if (displayName == nameof(IList.Remove))
                        return false;
                    if (displayName == nameof(IList.GetEnumerator))
                        return false;
                    if (displayName == nameof(IList.CopyTo))
                        return false;
                    if (displayName == nameof(IList.IndexOf))
                        return false;
                    if (displayName == nameof(IList.Contains))
                        return false;
                    if (displayName == nameof(IList.Add))
                        return false;
                    if (displayName == nameof(IList.RemoveAt))
                        return false;
                    if (displayName == nameof(ObservableCollection<object>.Move))
                        return false;
                    if (displayName == nameof(IList.Remove))
                        return false;
                    if (displayName == nameof(IList.Insert))
                        return false;
                }
                return true;
            }

            return true;
        }

        public static CustomFilter Instance { get; } = new();
    }
}
