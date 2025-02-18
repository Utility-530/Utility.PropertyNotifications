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
        public Character NewObject => new Character() { Color = Colors.Aqua };
    }


    public class ObjectToTokenConverter : IValueConverter
    {
        private JsonConverter[] converters = [
            new StringToGuidConverter(),
            //new Newtonsoft.Json.Converters.IsoDateTimeConverter(),
            new Newtonsoft.Json.Converters.StringEnumConverter()];


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return DependencyProperty.UnsetValue;

            var serialiser = JsonSerializer.CreateDefault(new JsonSerializerSettings { Converters = converters });
            return JToken.FromObject(value, serialiser);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}