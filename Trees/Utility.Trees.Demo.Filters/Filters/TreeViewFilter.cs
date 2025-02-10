using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Utility.Descriptors;
using Utility.Interfaces.NonGeneric;
using System.Collections.ObjectModel;
using Utility.Trees.WPF.Abstractions;
using Utility.Extensions;
using Utility.Trees.Abstractions;
using Utility.Helpers.NonGeneric;
using Utility.Trees.Decisions;

namespace Utility.Trees.Demo.Filters
{
    public class TreeViewFilter : ITreeViewFilter
    {
        public TreeViewFilter()
        {
            Predicate =
                 new TrueDecisionTree<IReadOnlyTree>(new Decision(item => true))
                 {
                     // prevents child items of a parent Descriptor that is being shown as a Treeviewer from also being shown
                     new TrueDecisionTree<IReadOnlyTree>(new Decision(item => ((IReadOnlyTree)item).Data as IPropertiesDescriptor != null))
                        {
                            new TrueDecisionTree<IReadOnlyTree>(new Decision(item => (((IReadOnlyTree)item).Data as IPropertiesDescriptor).Type == typeof(Table)))
                            {
                                new TrueDecisionTree<IReadOnlyTree>(new Decision(item => ((IReadOnlyTree)item).Parent == null), a => false)
                                {

                                },
                                new TrueDecisionTree<IReadOnlyTree>(new Decision(item => ((IReadOnlyTree)item).Parent != null), a => a.Parent)
                                {
                                    new TrueDecisionTree<IReadOnlyTree>(new Decision(item => ((IReadOnlyTree)item).Parent == null), a => false)
                                {

                                },
                                    new TrueDecisionTree<IReadOnlyTree>(new Decision(item => ((IReadOnlyTree)item).Parent != null),a=>a.Parent)
                                    {
                                        new TrueDecisionTree<IReadOnlyTree>(new Decision(item => ((IReadOnlyTree)item).Data as ICollectionDescriptor ==null), a => false),
                                    }
                                }
                            }
                        },

                      new TrueDecisionTree<IReadOnlyTree>(new Decision(item => true) { }, a => a.Data)
                    {
                        new TrueDecisionTree(new Decision(item => item as IMethodDescriptor!=null){  })
                        {
                            new TrueDecisionTree(new Decision<IMethodDescriptor>(md=>md.Type.IsArray==false),  a=> true)
                        },
                        new TrueDecisionTree(new Decision(item => item as IDescriptor != null && (item as IDescriptor).ParentType!=null))
                        {
                            new TrueDecisionTree(new Decision<IDescriptor>(item => item as IPropertiesDescriptor!=null))
                            {
                                new TrueDecisionTree(new Decision<IDescriptor>(item => item.Type.IsAssignableTo(typeof(IEnumerable))),  a=> false)
                            },
                            new TrueDecisionTree(new Decision<IDescriptor>(item => item as ICollectionItemDescriptor!=null))
                            {
                                new TrueDecisionTree(new Decision<IDescriptor>(item => (item as ICollectionItemDescriptor).Removed != null ),  a=> false)
                            },
                            new TrueDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.ParentType.Name == "Array"), item => item.Name)
                            {
                                new TrueDecisionTree(new Decision<string>(item => item == "IsFixedSize"),  a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "IsReadOnly"),  a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "IsSynchronized"),  a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "LongLength"),  a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "Length"), a=> false) { },
                                new TrueDecisionTree(new Decision<string>(item => item == "SyncRoot"),  a=> false) { },
                            },
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
