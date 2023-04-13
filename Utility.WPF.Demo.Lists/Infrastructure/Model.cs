using System.ComponentModel;
using System.Windows;

namespace Utility.WPF.Demo.Buttons.Infrastructure
{
    public class Model
    {
        [Description("One")]
        public void ShowOne()
        {
            MessageBox.Show("One");
        }

        [Description("Two")]
        public void ShowTwo()
        {
            MessageBox.Show("Two");
        }

        [Description("Three")]
        public void ShowThree()
        {
            MessageBox.Show("Three");
        }
    }
}