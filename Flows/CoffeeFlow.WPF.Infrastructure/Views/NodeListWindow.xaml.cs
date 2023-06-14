using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CoffeeFlow.Nodes;
using UnityFlow;
using Utility.Commands;
using Utility.Collections;
using CoffeeFlow.WPF.Infrastructure;

namespace CoffeeFlow.Views
{
    /// <summary>
    /// Interaction logic for NodeListWindow.xaml
    /// </summary>
    public partial class NodeListWindow : UserControl, INotifyPropertyChanged
    {
        public bool IsSetup = false;

        Style bluestyle = Application.Current.FindResource("BlueButton") as Style;
        Style darkstyle = Application.Current.FindResource("DarkButton") as Style;

        public NodeListWindow()
        {
            InitializeComponent();

        }

        private Command _showTriggers;
        public ICommand ShowTriggersCommand
        {
            get { return _showTriggers ?? (_showTriggers = new Command(ShowTriggers)); }
        }

        private Command _showMethods;

        public ICommand ShowMethodsCommand
        {
            get { return _showMethods ?? (_showMethods = new Command(ShowMethods)); }
        }

        private Command _showVariables;

        public ICommand ShowVariablesCommand
        {
            get { return _showVariables ?? (_showVariables = new Command(ShowVariables)); }
        }

        public Collection SearchCollection;

        public void ShowTriggers()
        {
            btnTriggers.Style = bluestyle;
            btnEvents.Style = darkstyle;
            btnInsight.Style = darkstyle;
            btnVariables.Style = darkstyle;

            //MainViewModel m = SimpleIoc.Default.GetInstance<MainViewModel>();
            lstAvailableNodes.ItemsSource = Globals.Instance.Main.NodesViewModel.Triggers;

            DisableSearch();
        }

        public void ShowMethods()
        {
            btnTriggers.Style = darkstyle;
            btnEvents.Style = darkstyle;
            btnInsight.Style = bluestyle;
            btnVariables.Style = darkstyle;

            //MainViewModel m = SimpleIoc.Default.GetInstance<MainViewModel>();
            lstAvailableNodes.ItemsSource = Globals.Instance.Main.NodesViewModel.Methods;

            DisableSearch();
        }

        public void ShowVariables()
        {
            btnTriggers.Style = darkstyle;
            btnEvents.Style = darkstyle;
            btnInsight.Style = darkstyle;
            btnVariables.Style = bluestyle;

            //MainViewModel m = SimpleIoc.Default.GetInstance<MainViewModel>();
            lstAvailableNodes.ItemsSource = Globals.Instance.Main.NodesViewModel.Variables;
            DisableSearch();
        }

        public void AddRootNode(string name)
        {
            RootNode root = new RootNode();
            root.Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        ObservableCollection<NodeWrapper> searchCopy = new();
        bool isSearching = false;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isSearching == false)
            {
                //initiate a search
                searchCopy = lstAvailableNodes.ItemsSource as ObservableCollection<NodeWrapper>;
                isSearching = true;
            }
            
            if(isSearching)
            {
                string searchText = searchBox.Text;
                var result = from node in searchCopy
                             where node.NodeName.ToLower().Contains(searchText.ToLower())
                             select node;

                lstAvailableNodes.ItemsSource = result;
            }
        }

        public void DisableSearch()
        {
            isSearching = false;
            searchBox.Text = "";
        }

        private void nodeList_Loaded(object sender, RoutedEventArgs e)
        {
            if(!IsSetup)
            {
                SearchCollection = new();
                ShowTriggers();
            }
        }

        public static readonly DependencyProperty IsCenterProperty = DependencyProperty.Register(
    "IsCenter", typeof(bool), typeof(NodeListWindow));

        public bool IsCenter
        {
            get { return (bool)GetValue(IsCenterProperty); }
            set
            {
                SetValue(IsCenterProperty, value);

            }
        }
    }
}
