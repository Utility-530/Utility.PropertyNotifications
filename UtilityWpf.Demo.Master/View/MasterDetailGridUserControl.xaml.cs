using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Endless;
using NetFabric.Hyperlinq;
using Utility.Common;
using Utility.WPF.Demo.Common.Meta;
using Utility.WPF.Demo.Common.ViewModels;

namespace UtilityWpf.Demo.Master.View
{
    /// <summary>
    /// Interaction logic for PersistListUserControl.xaml
    /// </summary>
    public partial class MasterDetailGridUserControl : UserControl
    {
        public MasterDetailGridUserControl()
        {
            InitializeComponent();
            //(this.DataContext as PersistListViewModel).Data = PersistBehavior.Items;
        }

        private void DragablzItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }

    public class CollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Resolver.Instance.Resolve<Factory>().Create<Fields>(Statics.Random.Next(10, 20)).Cached();

            }
            catch
            {
                return new[] { new Fields() };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static CollectionConverter Instance { get; } = new CollectionConverter();
    }
}