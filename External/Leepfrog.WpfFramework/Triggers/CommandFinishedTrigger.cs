using Leepfrog.WpfFramework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;


namespace Leepfrog.WpfFramework.Triggers
{
    // Custom DataTrigger, specifically for selecteditem of tabcontrol
    public class CommandFinishedTrigger : Microsoft.Xaml.Behaviors.EventTrigger
    {

        public CommandFinishedTrigger()
        {
            // just setup a dummy binding to point to the Command's Finished event
            var binding = new Binding("Command");
            binding.Source = this;
            BindingOperations.SetBinding(this,SourceObjectProperty,binding);

            // trigger when IsProcessing changes to true
            EventName = "Finished";
        }

        public AsyncRelayCommand Command
        {
            get { return (AsyncRelayCommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(AsyncRelayCommand), typeof(CommandFinishedTrigger), new PropertyMetadata(null));


    }
}

