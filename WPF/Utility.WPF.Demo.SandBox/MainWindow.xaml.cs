using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Commands;
using Utility.WPF.Controls.Objects;
using System.Windows.Data;
using Utility.WPF.Demo.Data.Model;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Utility.WPF.Demo.SandBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Character newObject = new Character() { Color = Colors.Aqua };

        public MainWindow()
        {
            InitializeComponent();
            FinishEdit = new Command<object>(o =>
            {
                if (o is NewObjectRoutedEventArgs args)
                {
                    Items.Add(args.NewObject as Character);
                }
            });

            var res = new Utility.WPF.Demo.Data.Resources();
            Items = new (res["Characters"] as Character[]);

            MainGrid.DataContext = this;
        }

        public ICommand FinishEdit { get; }

        public ObservableCollection<Character> Items { get; }
        public Character NewObject
        {
            get => newObject; set
            {
                newObject = value;
            }
        }
    }



}