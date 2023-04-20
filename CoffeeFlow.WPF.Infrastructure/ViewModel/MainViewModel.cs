using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using CoffeeFlow.Views;
using System.Collections;
using Utility.Collections;
using Utility.Commands;
using System.Windows.Input;
using Newtonsoft.Json;
using UnityFlow;
using CoffeeFlow.WPF.Infrastructure.Infrastructure;

namespace CoffeeFlow.ViewModel
{



    public class NodesViewModel
    {
        private Collection triggers = new(), methods = new(), variables = new();

        public NodesViewModel()
        {
            triggers.AddRange(NodeWrapperFactory.GetTriggers());
        }


        public Collection Triggers => triggers;

        public Collection Methods => methods;

        public Collection Variables => variables;

        internal void Initialise(bool isAppend, string file, bool isClassFileName)
        {
            if (!isAppend)
            {
                Methods.Clear();
                Variables.Clear();
            }

            Methods.AddRange(NodeWrapperFactory.GetMethods(file, isClassFileName));
            Variables.AddRange(NodeWrapperFactory.GetVariables(file, isClassFileName));
        }
    }


    /**********************************************************************************************************
      *             Logic related to the main window, window logic, code parsing and node list. 
      * 
      *                                                      * Nick @ http://immersivenick.wordpress.com 
      *                                                      * Free for non-commercial use
      * *********************************************************************************************************/
    public class MainViewModel : Jellyfish.ViewModel
    {
        private Command _OpenCodeWindowCommand, _OpenCodeFileFromFileCommand, _OpenLocalizationWindowCommand, _OpenDebug,
            _OpenLocalizationFile, _DeleteNodeFromNodeListCommand, _AddTriggerCommand;

        private Collection localizationStrings = new();

        private string statusLabel = null, fileLoadInfo = string.Empty;
        private bool isClassFileName = true, isAppend = true;



        public MainViewModel()
        {

            DebugList = new ObservableCollection<string>();
            LogStatus("MainViewModel initialized.");


        }

        public IEnumerable LocalizationStrings => localizationStrings;

        public ObservableCollection<string> DebugList { get; set; }

        public string StatusLabel
        {
            get => statusLabel;
            set => Set(ref statusLabel, value);
        }

        public string FileLoadInfo
        {
            get => fileLoadInfo;
            set => Set(ref fileLoadInfo, value);
        }

        public bool IsClassFileName
        {
            get => isClassFileName;
            set => Set(ref isClassFileName, value);
        }

        public bool IsAppend
        {
            get => isAppend;
            set => Set(ref isAppend, value);
        }

        public ICommand OpenCodeWindowCommand => _OpenCodeWindowCommand ??= new Command(openCodeLoadPanelUI);
        public ICommand OpenCodeFileFromFileCommand => _OpenCodeFileFromFileCommand ??= new Command(openCodeFromFile);
        public ICommand OpenLocalizationWindowCommand => _OpenLocalizationWindowCommand ??= new Command(openLocalizationLoadPanelUI);
        public ICommand OpenLocalizationFile => _OpenLocalizationFile ??= new Command(OpenLocalization);


        public ICommand OpenDebugCommand
        {
            get { return _OpenDebug ?? (_OpenDebug = new Command(OpenDebug)); }
        }

        private string _newTriggerName = "Enter trigger name";
        public string NewTriggerName
        {
            get { return _newTriggerName; }
            set
            {
                _newTriggerName = value;
                Set(ref _newTriggerName, value);
            }
        }

        public NodesViewModel NodesViewModel { get; } = new NodesViewModel(); 

        public ICommand AddTriggerCommand => _AddTriggerCommand ??= new Command(AddNewTrigger);

        //public ICommand DeleteNodeFromNodeListCommand => _DeleteNodeFromNodeListCommand ??= new Command(DeleteNodeFromNodeList);


        public void AddNewTrigger()
        {
            if (NewTriggerName != "")
            {
                NodeWrapper newTrigger = new NodeWrapper();
                newTrigger.NodeName = NewTriggerName;
                newTrigger.TypeOfNode = NodeType.RootNode;

                NodesViewModel.Triggers.Add(newTrigger);

                NewTriggerName = "";
            }
        }

        private void openLocalizationLoadPanelUI()
        {
            //OpenLocalizationData w = new OpenLocalizationData();
            //w.ShowDialog();
        }

        private void openCodeLoadPanelUI()
        {
            LogStatus("Ready to parse a C# code file", true);
            OpenCodeWindow w = new OpenCodeWindow();
            w.ShowDialog();
        }


        private void openCodeFromFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "C# Files (.cs)|*.cs|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            //openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            int added = 0;
            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                foreach (var file in openFileDialog1.FileNames)
                {
                    OpenCode(file);
                    added++;
                }

                FileLoadInfo = "Added " + added + " code file(s)";
            }
        }

        private void OpenCode(string file)
        {
            //Path.GetFileNameWithoutExtension(file);
            NodesViewModel.Initialise(IsAppend, file, isClassFileName);
  

            string className = Path.GetFileNameWithoutExtension(file);
            LogStatus("C# file " + className + ".cs parsed succesfully", true);
        }

        private void OpenLocalization()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "JSON Files (.json)|*.json|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            //openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            int added = 0;
            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                string path = openFileDialog1.FileName;
                string json = File.ReadAllText(path);
                try
                {
                    var data = JsonConvert.DeserializeObject<LocalizationData>(json);
                    localizationStrings.Clear();

                    foreach (var item in data.Items)
                    {
                        localizationStrings.Add(item);
                    }

                    LogStatus("Succesfully parsed " + localizationStrings.Count + " localization keys", true);
                }
                catch (Exception e)
                {
                    LogStatus("Json File is not in the correct format", true);
                    throw;
                }
            }
        }


        public void OpenDebug()
        {
            CodeResultWindow w = new CodeResultWindow();
            w.Show();
        }

    
        public void LogStatus(string status, bool showInStatusLabel = false)
        {
            DebugList.Add(status);

            //log
            if (showInStatusLabel)
                StatusLabel = status;

        }
    }
}