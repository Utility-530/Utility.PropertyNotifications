using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Utility.ViewModels;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for KeyValueCollectionUserControl.xaml
    /// </summary>
    public partial class KeyValueCollectionUserControl : UserControl
    {
        // public event Action<int> numberEvent;

        public KeyValueCollectionUserControl()
        {
            this.InitializeComponent();
            int number = 2;

            foreach (var (x_, y_) in this.SelectData())
            {
                this.Values.Add(x_, y_);
            }

            this.Values.KeyValueChanged += (a, b) => this.Act(number);
            this.DataGrid1.ItemsSource = this.Values;

            Act(number);
        }

        public ObservablePairCollection<double, double> Values { get; } = new ObservablePairCollection<double, double>();

        private void Act(int number)
        {
            //MessageBox.Show("Key value changed");
        }

        private IEnumerable<(int, int)> SelectData()
        {
            var ran = new Random();
            return Enumerable.Range(0, 10).Select(a => (a, ran.Next(20)));
        }
    }
}

// static void a()
// {
// REngine.SetEnvironmentVariables();
// REngine engine = REngine.GetInstance();
// // REngine requires explicit initialization.
// // You can set some parameters.
// engine.Initialize();

// // .NET Framework array to R vector.
// NumericVector group1 = engine.CreateNumericVector(new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
// engine.SetSymbol("group1", group1);
// // Direct parsing from R script.
// NumericVector group2 = engine.Evaluate("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric();

// // Test difference of mean and get the P-value.
// GenericVector testResult = engine.Evaluate("t.test(group1, group2)").AsList();
// double p = testResult["p.value"].AsNumeric().First();

// Console.WriteLine("Group1: [{0}]", string.Join(", ", group1));
// Console.WriteLine("Group2: [{0}]", string.Join(", ", group2));
// Console.WriteLine("P-value = {0:0.000}", p);

// // you should always dispose of the REngine properly.
// // After disposing of the engine, you cannot reinitialize nor reuse it
// engine.Dispose();

// }