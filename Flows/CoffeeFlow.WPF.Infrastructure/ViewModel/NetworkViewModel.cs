using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using CoffeeFlow.Base;
using CoffeeFlow.Nodes;
using Application = System.Windows.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using UnityFlow;
using Utility.Commands;
using CoffeeFlow.WPF.Infrastructure;

namespace CoffeeFlow.ViewModel
{
    /**********************************************************************************************************
   *             Logic related to the nodes present on the grid and their connections and related commands
   * 
   *                                                      * Nick @ http://immersivenick.wordpress.com 
   *                                                      * Free for non-commercial use
   * *********************************************************************************************************/
    public class NetworkViewModel : Jellyfish.ViewModel
    {
        private ObservableCollection<NodeViewModel> nodes = null;
        private int bezierStrength = 80;

        private Command<NodeWrapper> _addNodeToGridCommand;

        public NetworkViewModel()
        {
            AddedNodesOrder = new List<NodeViewModel>();
            RemovedNodesOrder = new List<NodeViewModel>();
        }


        public Stack<ICommand> UndoStack;
        public List<NodeViewModel> AddedNodesOrder;
        public List<NodeViewModel> RemovedNodesOrder;
        public int UndoCount = 0;

        public ICommand AddNodeToGridCommand
        {
            get { return _addNodeToGridCommand ?? (_addNodeToGridCommand = new Command<NodeWrapper>(AddNodeToGrid)); }
        }

        //private Command<LocalizationItem> _selectLocalizedStringCommand;
        //public ICommand SelectLocalizedStringCommand
        //{
        //    get { return _selectLocalizedStringCommand ?? (_selectLocalizedStringCommand = new Command<LocalizationItem>(SelectLocalizedString)); }
        //}

        private Command _SaveNodesCommand;
        public ICommand SaveNodesCommand
        {
            get { return _SaveNodesCommand ?? (_SaveNodesCommand = new Command(SaveNodes)); }
        }

        private Command _ClearNodesCommand;
        public ICommand ClearNodesCommand
        {
            get { return _ClearNodesCommand ?? (_ClearNodesCommand = new Command(ClearNodes)); }
        }


        private Command _LoadNodesCommand;
        public ICommand LoadNodesCommand
        {
            get { return _LoadNodesCommand ?? (_LoadNodesCommand = new Command(LoadNodes)); }
        }

        private Command<NodeViewModel> _deleteNodesCommand;
        public ICommand DeleteNodesCommand
        {
            get { return _deleteNodesCommand ?? (_deleteNodesCommand = new Command<NodeViewModel>(DeleteSelectedNodes)); }
        }

        //public DependencyObject MainWindow;
        public int BezierStrength { get => bezierStrength; set { bezierStrength = value; this.OnPropertyChanged(); } }

        private Command increaseBezier;
        public ICommand IncreaseBezierStrengthCommand
        {
            get { return increaseBezier ?? (increaseBezier = new Command(IncreaseBezier)); }
        }


        private Command decreaseBezier;
        public ICommand DecreaseBezierStrengthCommand
        {
            get { return decreaseBezier ?? (decreaseBezier = new Command(DecreaseBezier)); }
        }

        private Command resetBezier;
        public ICommand ResetBezierStrengthCommand
        {
            get { return resetBezier ?? (resetBezier = new Command(ResetBezier)); }
        }

        public void IncreaseBezier()
        {
            BezierStrength += 40;

        }

        public void DecreaseBezier()
        {
            BezierStrength -= 40;
        }

        public void ResetBezier()
        {
            BezierStrength = 80;
        }

        public void ConnectPinsFromSourceID(int sourceID, int destinationID)
        {
            Connector source = GetConnectorWithID(sourceID);
            Connector destination = GetConnectorWithID(destinationID);

            Connector.ConnectPins(source, destination);
        }

        //public void SelectLocalizedString(LocalizationItem s)
        //{
        //    System.Windows.Controls.TextBox t = DynamicNode.GetCurrentEditTextBox();
        //    if (t != null)
        //        t.Text = s.Key;
        //}

        public void AddNodeToGrid(NodeWrapper node)
        {
            NodeViewModel nodeToAdd = null;

            //Determine location to place node, bit of a hack
            MainWindow main = Application.Current.MainWindow as MainWindow;
            Point p = new Point(main.Width / 2, main.Height / 2);

            if (main.IsNodePopupVisible)
                p = Mouse.GetPosition(main);
            else
            {
                Random r = new Random();
                int increment = r.Next(-400, 400);
                p = new Point(p.X + increment, p.Y + increment);
            }


            if (node.TypeOfNode == NodeType.RootNode)
            {
                RootNode n = new RootNode();
                n.NodeName = node.NodeName;
     
                //n.Transform = new Thickness(p.X, p.Y, 0, 0);
                nodeToAdd = n;
            }

            if (node.TypeOfNode == NodeType.ConditionNode)
            {
                ConditionNode n = new ConditionNode();
                n.NodeName = node.NodeName;

                //    n.Margin = new Thickness(p.X, p.Y, 0, 0);
                nodeToAdd = n;
            }


            if (node.TypeOfNode == NodeType.MethodNode)
            {
                DynamicNode n = new DynamicNode();
                n.NodeName = node.NodeName;

                foreach (var arg in node.Arguments)
                {
                    n.AddArgument(arg.ArgTypeString, arg.Name, false, 0, null);
                }

                //  n.Margin = new Thickness(p.X, p.Y, 0, 0);
                n.CallingClass = node.CallingClass;
                nodeToAdd = n;
            }

            if (node.TypeOfNode == NodeType.VariableNode)
            {
                VariableNode n = new VariableNode();
                n.NodeName = node.NodeName;
                n.Type = node.BaseAssemblyType;

                // n.Margin = new Thickness(p.X, p.Y, 0, 0);
                n.CallingClass = node.CallingClass;
                nodeToAdd = n;
            }

            nodeToAdd.MoveBy(p.X, p.Y);

            if (nodeToAdd != null)
            {
                this.Nodes.Add(nodeToAdd);
                // MainViewModel.Instance.LogStatus("Added node " + nodeToAdd.NodeName + " to grid");
            }
            else
            {
                //  MainViewModel.Instance.LogStatus("Couldn't add node " + node.NodeName + " to grid");
            }

            //Close the node view window
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow.HideNodeListCommand.Execute(null);
        }

        public Connector GetConnectorWithID(int id)
        {
            Connector connector = null;

            var MainWindow = Globals.Instance.MainWindow;

            if (MainWindow == null)
                return null;

            var Connectors = FindVisualChildren<Connector>(MainWindow);

            foreach (Connector cp in Connectors)
            {
                if (cp.ID == id)
                {
                    return cp;
                }
            }

            throw new Exception("dfg df");
        }

        public void ClearNodes()
        {
            this.Nodes.Clear();
            //MainViewModel.Instance.LogStatus("Cleared nodes.");
        }

        public void SaveNodes()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();

            // Set filter options and filter index.
            saveFileDialog1.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;

            string path = "";
            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = saveFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                path = saveFileDialog1.FileName;
                Stream myStream = saveFileDialog1.OpenFile();
                if (myStream != null)
                {
                    #region Nodes
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                    {
                        Indent = true,
                        IndentChars = "\t",
                        NewLineOnAttributes = true
                    };

                    List<SerializeableNodeViewModel> SerializeNodes = new List<SerializeableNodeViewModel>();

                    XmlSerializer serializer = new XmlSerializer(typeof(List<SerializeableNodeViewModel>), new Type[] { typeof(SerializeableVariableNode), typeof(SerializeableConditionNode), typeof(SerializeableRootNode), typeof(SerializeableDynamicNode) });
                    TextWriter writer = new StreamWriter(myStream);

                    foreach (var node in Nodes)
                    {
                        if (node is RootNode)
                        {
                            RootNode rootNode = node as RootNode;

                            SerializeableRootNode rootSerial = new SerializeableRootNode();
                            rootSerial.NodeName = rootNode.NodeName;
                            rootSerial.NodeType = rootSerial.NodeType;
                            rootSerial.MarginX = rootNode.Margin.Left + rootNode.Transform.X;
                            rootSerial.MarginY = rootNode.Margin.Top + rootNode.Transform.Y;
                            rootSerial.ID = rootNode.ID;
                            rootSerial.OutputNodeID = rootNode.OutputConnector.ConnectionNodeID; //connection to next node
                            rootSerial.CallingClass = rootNode.CallingClass;
                            SerializeNodes.Add(rootSerial);
                        }

                        if (node is ConditionNode)
                        {
                            ConditionNode conNode = node as ConditionNode;

                            SerializeableConditionNode conSerial = new SerializeableConditionNode();
                            conSerial.NodeName = conNode.NodeName;
                            conSerial.NodeType = conNode.NodeType;
                            conSerial.MarginX = conNode.Margin.Left + conNode.Transform.X;
                            conSerial.MarginY = conNode.Margin.Top + conNode.Transform.Y;
                            conSerial.ID = conNode.ID;
                            conSerial.InputNodeID = conNode.InExecutionConnector.ConnectionNodeID;
                            conSerial.OutputTrueNodeID = conNode.OutExecutionConnectorTrue.ConnectionNodeID;
                            conSerial.OutputFalseNodeID = conNode.OutExecutionConnectorFalse.ConnectionNodeID;
                            conSerial.BoolVariableID = conNode.boolInput.ConnectionNodeID;
                            conSerial.BoolVariableName = conNode.ConnectedToVariableName;
                            conSerial.BoolCallingClass = conNode.ConnectedToVariableCallerClassName;

                            SerializeNodes.Add(conSerial);
                        }

                        if (node is VariableNode)
                        {
                            VariableNode varNode = node as VariableNode;

                            SerializeableVariableNode varSerial = new SerializeableVariableNode();

                            varSerial.NodeName = varNode.NodeName;
                            varSerial.TypeString = varNode.Type;
                            varSerial.NodeType = varNode.NodeType;

                            varSerial.MarginX = varNode.Margin.Left + varNode.Transform.X;
                            varSerial.MarginY = varNode.Margin.Top + varNode.Transform.Y;
                            varSerial.ID = varNode.ID;

                            varSerial.ConnectedToNodeID = varNode.NodeParameterOut.ConnectionNodeID;
                            varSerial.ConnectedToConnectorID = varNode.NodeParameterOut.ConnectedToConnectorID;

                            varSerial.CallingClass = varNode.CallingClass;

                            SerializeNodes.Add(varSerial);
                        }

                        if (node is DynamicNode)
                        {
                            DynamicNode dynNode = node as DynamicNode;

                            SerializeableDynamicNode dynSerial = new SerializeableDynamicNode();

                            dynSerial.NodeType = dynNode.NodeType;
                            dynSerial.NodeName = dynNode.NodeName;
                            dynSerial.Command = dynNode.Command;
                            dynSerial.NodePanelHeight = dynNode.NodeHeight;
                            dynSerial.MarginX = dynNode.Margin.Left + dynNode.Transform.X;
                            dynSerial.MarginY = dynNode.Margin.Top + dynNode.Transform.Y;
                            dynSerial.ID = dynNode.ID;

                            foreach (var arg in dynNode.ArgumentCache)
                            {
                                dynSerial.Arguments.Add(arg);
                            }

                            dynSerial.InputNodeID = dynNode.InConnector.ConnectionNodeID;
                            dynSerial.OutputNodeID = dynNode.OutConnector.ConnectionNodeID;

                            dynSerial.CallingClass = dynNode.CallingClass;

                            SerializeNodes.Add(dynSerial);
                        }
                    }

                    serializer.Serialize(writer, SerializeNodes);
                    writer.Close();
                    myStream.Close();

                    System.Windows.Clipboard.SetText(path);
                    // MainViewModel.Instance.LogStatus("Save completed. Path: " + path + " copied to clipboard", true);
                    #endregion
                }
            }
        }

        public void LoadNodes()
        {
            ClearNodes();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                List<SerializeableNodeViewModel> SerializeNodes = new List<SerializeableNodeViewModel>();

                string path = openFileDialog1.FileName;

                XmlSerializer ser = new XmlSerializer(typeof(List<SerializeableNodeViewModel>), new Type[] { typeof(SerializeableVariableNode), typeof(SerializeableConditionNode), typeof(SerializeableDynamicNode), typeof(SerializeableRootNode) });

                using (XmlReader reader = XmlReader.Create(path))
                {
                    SerializeNodes = (List<SerializeableNodeViewModel>)ser.Deserialize(reader);
                }

                //ADD NODES
                foreach (var serializeableNodeViewModel in SerializeNodes)
                {
                    if (serializeableNodeViewModel is SerializeableRootNode)
                    {
                        SerializeableRootNode rootSerialized = serializeableNodeViewModel as SerializeableRootNode;

                        RootNode newNode = new RootNode();
                        newNode.Populate(rootSerialized);

                        Nodes.Add(newNode);
                    }

                    if (serializeableNodeViewModel is SerializeableVariableNode)
                    {
                        SerializeableVariableNode variableSerialized = serializeableNodeViewModel as SerializeableVariableNode;

                        VariableNode newNode = new VariableNode();
                        newNode.Populate(variableSerialized);

                        Nodes.Add(newNode);
                    }

                    if (serializeableNodeViewModel is SerializeableDynamicNode)
                    {
                        SerializeableDynamicNode dynamicSerialized = serializeableNodeViewModel as SerializeableDynamicNode;

                        DynamicNode newNode = new DynamicNode();
                        newNode.Populate(dynamicSerialized);

                        Nodes.Add(newNode);
                    }

                    if (serializeableNodeViewModel is SerializeableConditionNode)
                    {
                        SerializeableConditionNode conSerialized = serializeableNodeViewModel as SerializeableConditionNode;

                        ConditionNode newNode = new ConditionNode();
                        newNode.Populate(conSerialized);

                        Nodes.Add(newNode);
                    }
                }
            }

            //Node Connections
            foreach (var node in Nodes)
            {
                if (node is RootNode)
                {
                    //Connect output
                    RootNode rootNode = node as RootNode;
                    if (rootNode.OutputConnector.ConnectionNodeID <= 0)
                        return;

                    //Connect this output to the connection's input
                    Connector connectedTo = GetInConnectorBasedOnNode(rootNode.OutputConnector.ConnectionNodeID);
                    Connector.ConnectPins(rootNode.OutputConnector, connectedTo);
                }

                if (node is ConditionNode)
                {
                    ConditionNode conNode = node as ConditionNode;

                    //bool value Input
                    if (conNode.boolInput.ConnectionNodeID > 0)
                    {
                        //we're connected to a parameter
                        Connector connectedToVar = GetOutConnectorBasedOnNode(conNode.boolInput.ConnectionNodeID); //variable
                        Connector.ConnectPins(conNode.boolInput, connectedToVar);

                    }

                    //Input
                    if (conNode.InExecutionConnector.ConnectionNodeID > 0)
                    {
                        Connector connectedTo = GetOutConnectorBasedOnNode(conNode.InExecutionConnector.ConnectionNodeID);
                        Connector.ConnectPins(conNode.InExecutionConnector, connectedTo);
                    }

                    //Ouput true
                    if (conNode.OutExecutionConnectorTrue.ConnectionNodeID > 0)
                    {
                        Connector connectedTo = GetInConnectorBasedOnNode(conNode.OutExecutionConnectorTrue.ConnectionNodeID);
                        Connector.ConnectPins(conNode.OutExecutionConnectorTrue, connectedTo);
                    }

                    //Ouput false
                    if (conNode.OutExecutionConnectorFalse.ConnectionNodeID > 0)
                    {
                        Connector connectedTo = GetInConnectorBasedOnNode(conNode.OutExecutionConnectorFalse.ConnectionNodeID);
                        Connector.ConnectPins(conNode.OutExecutionConnectorFalse, connectedTo);
                    }
                }

                if (node is DynamicNode)
                {
                    //Connect output
                    DynamicNode dynNode = node as DynamicNode;

                    //Connect parameters
                    for (int i = 0; i < dynNode.ArgumentCache.Count(); i++)
                    {
                        Argument arg = dynNode.ArgumentCache.ElementAt(i);

                        if (arg.ArgIsExistingVariable)
                        {
                            Connector conID = dynNode.GetConnectorAtIndex(i);
                            int connectedToVar = arg.ArgumentConnectedToNodeID;

                            Connector varConnect = GetOutConnectorBasedOnNode(connectedToVar);
                            Connector.ConnectPins(conID, varConnect);
                        }
                    }

                    if (dynNode.OutExecutionConnector.ConnectionNodeID > 0)
                    {
                        //Connect this output to the connection's input
                        Connector connectedTo = GetInConnectorBasedOnNode(dynNode.OutExecutionConnector.ConnectionNodeID);
                        Connector.ConnectPins(dynNode.OutExecutionConnector, connectedTo);
                        //No need to connect this in to the connection's output so far.
                    }
                }
            }
        }

        //finds children
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }


        public ObservableCollection<NodeViewModel> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new ObservableCollection<NodeViewModel>();
                }

                return nodes;
            }
        }

        public static ObservableCollection<NodeViewModel> SelectedNodes { get; set; }

        public void DeleteSelectedNodes(NodeViewModel node)
        {
            if (node != null)
            {
                node.DisconnectAllConnectors();
                Nodes.Remove(node);
            }
        }

        public Connector GetInConnectorBasedOnNode(int nodeID)
        {
            NodeViewModel node = GetNodeByID(nodeID);

            if (node == null)
                return null;

            if (node is DynamicNode)
            {
                var dyn = node as DynamicNode;
                return dyn.InConnector;
            }

            if (node is ConditionNode)
            {
                var con = node as ConditionNode;
                return con.InExecutionConnector;
            }

            return null;
        }

        public Connector GetOutConnectorBasedOnNode(int nodeID)
        {
            NodeViewModel node = GetNodeByID(nodeID);

            if (node == null)
                return null;

            if (node is RootNode)
            {
                var root = node as RootNode;
                return root.OutputConnector;
            }

            if (node is DynamicNode)
            {
                var dyn = node as DynamicNode;
                return dyn.OutConnector;
            }

            if (node is VariableNode)
            {
                var con = node as VariableNode;
                return con.NodeParameterOut;
            }

            return null;
        }

        public NodeViewModel GetNodeByID(int nodeID)
        {
            var node = from n in Nodes
                       where n.ID == nodeID
                       select n;

            if (node.Count() == 1)
            {
                return node.First();
            }

            return null;
        }

    }
}
