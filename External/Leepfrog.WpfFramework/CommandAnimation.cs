using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace Leepfrog.WpfFramework
{

    // NOTES:
    // this was supposed to allow us to fire a command at any point during an animation
    // at the moment, it only works when the ENTIRE storyboard is complete
    // so even if this is a child storyboard, the parent and all of its children must complete for this to fire


    public class CommandAnimation : Timeline
    {

        public CommandAnimation() : base()
        {
            Duration = new Duration(TimeSpan.FromMilliseconds(1));
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandAnimation));

        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandAnimation));

    public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }
            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }



        public string Debug
        {
            get { return (string)GetValue(DebugProperty); }
            set { SetValue(DebugProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Debug.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DebugProperty =
            DependencyProperty.Register("Debug", typeof(string), typeof(CommandAnimation), new PropertyMetadata(null));



        private void addDebugLog(string message)
        {
            if (string.IsNullOrEmpty(Debug))
            {
                return;
            }
            else
            {
                this.AddLog("ANI", $"{message} ({ Debug })");
            }
        }





        protected override Clock AllocateClock()
        {
            addDebugLog("CommandAnimation Creating Clock");
            var clock = base.AllocateClock();
            clock.Completed += clock_Completed;
            clock.RemoveRequested += clock_RemoveRequested;
            return clock;
        }

        void clock_RemoveRequested(object sender, EventArgs e)
        {
        }

        void clock_Completed(object sender, EventArgs e)
        {
            //called when animation 'completes' 
            //we will use this to execute our command
            var command = Command;
            if (command == null)
            {
                addDebugLog("CommandAnimation Triggered, but command is null");
            }
            else
            {
                addDebugLog("CommandAnimation Triggered");
                command.Execute(CommandParameter);
            }
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            addDebugLog("CommandAnimation CreateInstanceCore");
            return new CommandAnimation();
        }


    }

}
