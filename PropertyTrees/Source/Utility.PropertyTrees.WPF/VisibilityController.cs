using NetFabric.Hyperlinq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Utility.Infrastructure;
using Utility.Models;
using Utility.Trees;

namespace Utility.PropertyTrees.Services
{
    public class VisibilityController : BaseObject
    {
        Dictionary<Key, TreeViewItem> treeViewItems = new();

        public List<ConditionalAction> keyValuePairs = new List<ConditionalAction>() { new ShowViewModels(), new HideViewModels() };

        public override Key Key { get; } = new Key(Guids.VisibilityController, nameof(VisibilityController), typeof(VisibilityController));

        public void OnNext(TreeViewItemInitialised change)
        {
            if (change is { Source: TreeViewItem treeViewItem, Node: { Key: Key key } })
            {
                treeViewItems.Add(key, treeViewItem);
            }

            _OnNext(change);
        }

        public void OnNext(SelectionChange change)
        {
            _OnNext(change);
        }

        private void _OnNext(Event @event)
        {
            foreach (var conditionalAction in keyValuePairs)
            {
                if (conditionalAction.EventPredicate(@event))
                {
                    foreach (var treeViewItem in treeViewItems)
                    {
                        if (conditionalAction.KeyPredicate(treeViewItem.Key))
                        {
                            conditionalAction.Action(treeViewItem.Value);
                        }
                    }
                }
            }
        }


        public abstract class ConditionalAction
        {
            public abstract bool EventPredicate(Event @event);
            public abstract bool KeyPredicate(Key @event);
            public abstract void Action(TreeViewItem @event);
        }

        public class ShowViewModels : ConditionalAction
        {
            public override void Action(TreeViewItem @event)
            {
                @event.Visibility = System.Windows.Visibility.Visible;
            }

            public override bool EventPredicate(Event @event)
            {
                if (@event is SelectionChange { Node: PropertyBase node } change)
                {
                    return node.Ancestors().Cast<PropertyBase>().Any(a => a.Name == "ViewModels") == false;
                }
                return false;
            }

            public override bool KeyPredicate(Key @event)
            {
                return @event.Name == "ViewModels";
            }
        }

        public class HideViewModels : ConditionalAction
        {
            public override void Action(TreeViewItem @event)
            {
                @event.Visibility = System.Windows.Visibility.Collapsed;
            }

            public override bool EventPredicate(Event @event)
            {
                if (@event is TreeViewItemInitialised { Node: PropertyBase node } change)
                {
                    return node.Name == "ViewModels";
                }
                return false;
            }

            public override bool KeyPredicate(Key @event)
            {
                return @event.Name == "ViewModels";
            }
        }

    }
}