using System.Collections;
using System.Collections.ObjectModel;
using Utility;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;

namespace Utility.WPF.Trees
{
    public class TreeViewFilter : IFilter
    {
        public TreeViewFilter()
        {
            Predicate =
                 new TrueDecisionTree<IReadOnlyTree>(new Decision(item => true))
                 {
                      new TrueDecisionTree<IReadOnlyTree>(new Decision(item => true) { }, a => a)
                    {
                        new TrueDecisionTree(new Decision(item => item as IMethodDescriptor!=null){  })
                        {
                            new TrueDecisionTree(new Decision<IMethodDescriptor>(md=>md.Type.IsArray==false),  a=> true)
                        },
                        new TrueDecisionTree(new Decision(item => item as IDescriptor != null && (item as IDescriptor).ParentType!=null))
                        {
                            //new TrueDecisionTree(new Decision<IDescriptor>(item => item as ICollectionItemDescriptor!=null))
                            //{
                            //    //new TrueDecisionTree(new Decision<IDescriptor>(item => (item as ICollectionItemDescriptor).Removed != null ),  a=> false)
                            //},
                            new TrueDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.ParentType.Name == "BitmapImage"), item =>false),
                            new TrueDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.Name == "DependencyObjectType"), item =>false),
                            new TrueDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.Type.Name == "ImageSource"), item =>false),
                            new TrueDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.ParentType.Name == "Array"), item => item.Name)
                            {
                                new TrueDecisionTree(new Decision<string>(item => item == "IsFixedSize"),  a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "IsReadOnly"),  a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "IsSynchronized"),  a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "LongLength"),  a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "Length"), a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "SyncRoot"),  a=> false) { },
                            },
                            new TrueDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.Type.Name == "BitmapImage"),item => false),
                            new TrueDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.ParentType.Name == "String"),item =>item.Name)
                            {
                                new TrueDecisionTree(new Decision<string>(item => item == "Length"),  a=> true) { }
                            },
                            new TrueDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.ParentType.IsAssignableTo(typeof(IList))), item =>item.Name)
                            {
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.Remove)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.GetEnumerator)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.CopyTo)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.IndexOf)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.Contains)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.Add)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.RemoveAt)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(ObservableCollection<object>.Move))) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.Remove)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.Insert)),  a=> false) { },
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.Add)), a=> false),
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(IList.Count)), a=> false),
                            new TrueDecisionTree(new Decision<string>(item => item == nameof(List<object>.Capacity)), a=> false)
                        }
                        }
                      }
                };
        }

        public bool Filter(object item)
        {
            Predicate.Reset();
            Predicate.Input = item;
            Predicate.Evaluate();
            return (bool)(Predicate.Backput ?? true);
        }

        public DecisionTree Predicate { get; set; }

        public static TreeViewFilter Instance { get; } = new();
    }
}