using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using System.Windows.Input;
using Utility.Commands;
using Utility.Enums;
using Utility.Interfaces.Exs.Diagrams;
using Utility.PropertyNotifications;

namespace Utility.Nodes
{
    public class PendingConnectorViewModel : NodeViewModel, IConnectorViewModel, IPendingConnectorViewModel
    {
        private bool isDropDownOpen;
        private object data;

        public event Action<PropertyInfo> ConnectorAdded;

        public event Action<NotifyCollectionChangedEventArgs> ConnectorsChanged;

        public PendingConnectorViewModel()
        {
            AddConnectorCommand = new Command<object>((a) =>
            {
                if (a is PropertyInfo propertyInfo)
                {
                    //var x = new ConnectorViewModel() { Data = propertyInfo, Key = propertyInfo.Name, Node = this.Node, IsInput = true }
                    //this.Node.Input.Add();
                    ConnectorAdded.Invoke(propertyInfo);
                }
            });
            ChangeConnectorsCommand = new Command<object>((a) =>
            {
                if (a is CollectionChanges { Additions: IList { } additions, Removals: IList removals })
                {

                    if (additions is { Count: > 0 })
                        ConnectorsChanged?.Invoke(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, additions));

                    if (removals is { Count: > 0 })
                        ConnectorsChanged?.Invoke(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removals));
                }
            });
        }

        public bool IsDropDownOpen { get => isDropDownOpen; set => RaisePropertyChanged(ref isDropDownOpen, value); }
        public bool IsConnected { get; set; }
        public bool IsInput { get; set; }
        public INodeViewModel Node { get; set; }
        public PointF Anchor { get; set; }
        public IO Flow { get; }
        public FlatShape Shape { get; }

        public ICommand AddConnectorCommand { get; }
        public ICommand ChangeConnectorsCommand { get; }
    }

    public record CollectionChanges(IList Additions, IList Removals);
}