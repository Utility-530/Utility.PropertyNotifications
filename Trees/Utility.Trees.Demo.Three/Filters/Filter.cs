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

namespace Utility.Trees.Demo.MVVM
{
    internal class Filter : ITreeViewFilter
    {
        public Filter()
        {
            //var tre_e = new Tree() { new Decider(a=>true), new Decider(a => true) };

            Predicate = new BooleanDecisionTree<IReadOnlyTree>(new Decision(item => true) { }, a => a.Data)
                {
                    new BooleanDecisionTree(new Decision(item => item as IMethodDescriptor!=null){  })
                    {
                        new BooleanDecisionTree(new Decision<IMethodDescriptor>(md=>md.Type.IsArray==false),  a=> true)
                    },
                    new BooleanDecisionTree(new Decision(item => item as IDescriptor != null && (item as IDescriptor).ParentType!=null))
                    {
                          new BooleanDecisionTree(new Decision<IDescriptor>(item => item as IPropertiesDescriptor!=null))
                          {
                              new BooleanDecisionTree(new Decision<IDescriptor>(item => item.Type.IsAssignableTo(typeof(IEnumerable))),  a=> false)
                          },
                            new BooleanDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.ParentType.Name == "Array"), item => item.Name)
                            {
                                new BooleanDecisionTree( new Decision<string>(item => item == "IsFixedSize"),  a=> false) { },
                                new BooleanDecisionTree( new Decision<string>(item => item == "IsReadOnly"),  a=> false) { },
                                new BooleanDecisionTree( new Decision<string>(item => item == "IsSynchronized"),  a=> false) { },
                                new BooleanDecisionTree(new Decision<string>(item => item == "LongLength"),  a=> false) { },
                                new BooleanDecisionTree(new Decision<string>(item => item == "Length"), a=> false) { },
                                new BooleanDecisionTree(new Decision<string>(item => item == "SyncRoot"),  a=> false) { },
                            },
                            new BooleanDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.ParentType.Name == "String"),item =>item.Name)
                            {
                                new BooleanDecisionTree(new Decision<string>(item => item == "Length"),  a=> true) { }
                            },
                            new BooleanDecisionTree<IDescriptor>(new Decision<IDescriptor>(item => item.ParentType.IsAssignableTo(typeof(IList))), item =>item.Name)
                            {
                            new BooleanDecisionTree(new Decision<string>(item => item == nameof(IList.Remove)),  a=> false) { },
                            new BooleanDecisionTree(  new Decision<string>(item => item == nameof(IList.GetEnumerator)),  a=> false) { },
                            new BooleanDecisionTree(  new Decision<string>(item => item == nameof(IList.CopyTo)),  a=> false) { },
                            new BooleanDecisionTree(  new Decision<string>(item => item == nameof(IList.IndexOf)),  a=> false) { },
                            new BooleanDecisionTree(   new Decision<string>(item => item == nameof(IList.Contains)),  a=> false) { },
                            new BooleanDecisionTree(   new Decision<string>(item => item == nameof(IList.Add)),  a=> false) { },
                            new BooleanDecisionTree(   new Decision<string>(item => item == nameof(IList.RemoveAt)),  a=> false) { },
                            new BooleanDecisionTree(  new Decision<string>(item => item == nameof(ObservableCollection<object>.Move))) { },
                            new BooleanDecisionTree(   new Decision<string>(item => item == nameof(IList.Remove)),  a=> false) { },
                            new BooleanDecisionTree(  new Decision<string>(item => item == nameof(IList.Insert)),  a=> false) { },
                            new BooleanDecisionTree(  new Decision<string>(item => item == nameof(IList.Add)), a=> false)
                        }

                    }
                };

        }

        public bool Convert(object item)
        {
            Predicate.Reset();
            Predicate.Input = item;
            Predicate.Evaluate();
            return (bool)Predicate.Backput;
            //return true;
        }


        public DecisionTree Predicate { get; set; }

        public static Filter Instance { get; } = new();
    }
}
