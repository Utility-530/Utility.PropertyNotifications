using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Trees.Abstractions;

namespace Utility.Models.Trees
{
    public abstract class BreadCrumbModel : Model
    {
        public BreadCrumbModel()
        {
        }

        public override void Update(IReadOnlyTree current)
        {
            if (this == current)
                return;

            var topNode = this as INodeViewModel;
            var currentNode = current;

            List<IViewModelTree> items = new();
            IViewModelTree child = topNode;
            while (child.Current != null)
            {
                items.Add(child = child.Current);
                child.IsExpanded = false;
            }

            int i = 0;

            lock (this.Children)
                while (true)
                {
                    if (items.Count > i)
                    {

                        if (topNode.Count > i)
                        {
                            if (topNode[i] != items[i] && topNode[i].Equals(items[i]) == false)
                            {
                                topNode[i] = items[i];
                            }
                        }
                        else
                        {
                            try
                            {
                                topNode.Add(items[i]);
                            }
                            catch (InvalidOperationException ex) when (ex.Message == "Cannot change ObservableCollection during a CollectionChanged event.")
                            {
                                Utility.Globals.UI.Post(a => topNode.Add(a as INodeViewModel), items[i]);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }

            for (int j = topNode.Count - 1; j > items.Count - 1; j--)
            {
                topNode.RemoveAt(j);
            }
        }


        public override void Addition(IReadOnlyTree a)
        {
            if (this.Children.Count() == 1)
            {
                (this as INodeViewModel).Current = a as INodeViewModel;
            }
            base.Addition(a);
        }
    }
}
