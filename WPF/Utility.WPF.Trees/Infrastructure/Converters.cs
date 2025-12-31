using System.Collections;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using NodaTime;
using Utility;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Nodes;
using Utility.WPF.Trees;
using Utility.Interfaces;
using Utility.Trees.Extensions;
using MoreLinq;

namespace Utility.WPF.Trees
{


    public class AddObjectAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is EditRoutedEventArgs { IsAccepted: true, Edit: { } instance } value)
            {
                var x = AssociatedObject;
                ;
                if (x.DataContext is IChildren descriptor)
                {
                    if (descriptor.Children is IList list)
                    {
                        //if (instance is ISetKey setKey)
                        //    setKey.Key = (GuidKey)Guid.NewGuid();
                        if (instance is NodeViewModel nodeViewModel)
                        {
                            ChildrenConverter.Instance.source.Destroy(nodeViewModel.Key());
                            var (p, name) = ChildrenConverter.Instance.parentDict[nodeViewModel.Name()];
                            nodeViewModel.SuppressExceptions = true;
                            nodeViewModel.SetParent(p);
                            nodeViewModel.SetName(name);    
                            nodeViewModel.SetKey(null);
                  
                            nodeViewModel.SuppressExceptions = false;
                            nodeViewModel.Descendants().OfType<NodeViewModel>().ForEach(nvm =>
                            {
                                nvm.AreChildrenLoaded = false;
                                nvm.SuppressExceptions = true;                
                                nvm.SetKey(null);
                                nodeViewModel.SuppressExceptions = false;
                            });
                            nodeViewModel.AreChildrenLoaded = false;
                        }
                        list?.Add(instance);
                        //descriptor.OnNext(new System.ComponentModel.RefreshEventArgs(instance));
                    }
                }
            }
        }
    }

    public class CustomEventTrigger : Microsoft.Xaml.Behaviors.EventTrigger
    {
        public bool IsHandled { get; set; } = true;

        protected override void OnEvent(EventArgs eventArgs)
        {
            var routedEventArgs = eventArgs as RoutedEventArgs;
            if (routedEventArgs != null)
                routedEventArgs.Handled = IsHandled;

            base.OnEvent(eventArgs);
        }
    }
}