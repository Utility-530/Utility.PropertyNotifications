using DynamicData;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using Utility.WPF.Demo.Controls;
using Utility.WPF.Demo.Data.Model;

namespace Utility.WPF.Demo.View
{
    /// <summary>
    /// Interaction logic for DataGridUserControl.xaml
    /// </summary>
    public partial class DataGridVirtualisationUserControl : UserControl
    {
        public DataGridVirtualisationUserControl()
        {
            InitializeComponent();

            dataGridTest.ItemsSource = Enumerable.Range(0, 20).Select(a => new Stock { Key = "fds", Name = "sdf", Sector = "sdfvv" }).Cast<object>();

            var dc = new ProfileCollectionVirtualiseLimited(this.Behavior1.WhenAny(a => a.FirstIndex, b => (b.Sender.FirstIndex, b.Sender.LastIndex, b.Sender.Size))
           .Select(a => new VirtualRequest(a.FirstIndex, a.Size))
           .Skip(1)
           .StartWith(new VirtualRequest(0, 30)));

            Grid1.DataContext = dc;

            this.Behavior1.WhenAny(a => a.FirstIndex, b => (b.Sender.FirstIndex, b.Sender.LastIndex, b.Sender.Size)).Subscribe(s =>
            {
                var (first, last, size) = s;
                FirstIndexBox.Text = first.ToString();
                LastIndexBox.Text = last.ToString();
                SizeBox.Text = size.ToString();
            });

            var dc2 = new ProfileCollectionVirtualise(this.Behavior2.WhenAny(a => a.FirstIndex, b => (b.Sender.FirstIndex, b.Sender.LastIndex, b.Sender.Size))
                .Select(a => new VirtualRequest(a.FirstIndex, a.Size))
                .Skip(1)
                .StartWith(new VirtualRequest(0, 30)), initialSize: 1000);

            this.Behavior2.WhenAny(a => a.FirstIndex, b => (b.Sender.FirstIndex, b.Sender.LastIndex, b.Sender.Size)).Subscribe(s =>
              {
                  var (first, last, size) = s;
                  FirstIndexBox2.Text = first.ToString();
                  LastIndexBox2.Text = last.ToString();
                  SizeBox2.Text = size.ToString();
              });

            Grid2.DataContext = dc2;
        }
    }
}