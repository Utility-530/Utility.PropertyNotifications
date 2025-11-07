using System;
using System.Windows;
using Utility.WPF.Controls.Meta;

namespace Utility.WPF.Demo.Objects
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewControl(typeof(JsonObjectUserControl).Assembly)
            }.Show();
            base.OnStartup(e);
        }
    }

    public class ExceptionOne : Exception
    {
        public ExceptionOne() : base("On9j99999999999999 9  9 9ije")
        {
            Data.Add("One", "1");
        }

        public int One { get; set; }
    }

    public class ExceptionTwo : Exception
    {
        public ExceptionTwo() : base("Two")
        {
            Data.Add("Two", "2");
        }

        public int Two { get; set; }
    }
}