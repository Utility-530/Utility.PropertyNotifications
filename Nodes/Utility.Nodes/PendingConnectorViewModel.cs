using System.Collections;
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
    public class PendingConnectorViewModel : NotifyPropertyClass, IConnectorViewModel, IPendingConnectorViewModel
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
                if (a is (IEnumerable x, IEnumerable y))
                {
                    foreach (var item in x)
                    {
                        ConnectorsChanged?.Invoke(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)x));
                        return;
                    }
                    foreach (var item in y)
                    {
                        ConnectorsChanged?.Invoke(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)y));
                        return;
                    }

                    //foreach (PropertyInfo propertyInfo in x)
                    //{
                    //    var x = new ConnectorViewModel() { Data = propertyInfo, Key = propertyInfo.Name, Node = this.Node };

                    //    this.Node.Output.Add();
                    //}
                    //foreach (PropertyInfo propertyInfo in y)
                    //    this.Node.Output.RemoveBy(a => a.Data.Equals(propertyInfo));
                }
            });
        }

        public bool IsDropDownOpen { get => isDropDownOpen; set => RaisePropertyChanged(ref isDropDownOpen, value); }
        public bool IsConnected { get; set; }
        public object Data { get => data; set => RaisePropertyChanged(ref data, value); }
        public bool IsInput { get; set; }
        public INodeViewModel Node { get; set; }
        public IReadOnlyCollection<IConnectionViewModel> Connections { get; }
        public PointF Anchor { get; set; }
        public IO Flow { get; }
        public FlatShape Shape { get; }
        public string Key { get; }
        public Guid Guid { get; set; }

        public ICommand AddConnectorCommand { get; }
        public ICommand ChangeConnectorsCommand { get; }
    }
}