using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Commands;

namespace Utility.WPF.Behaviors
{
    public class GuidContextBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            var menuItem = new MenuItem
            {
                Header = "Generate",
                Command = new Command(() =>
                {
                    AssociatedObject.Text = Guid.NewGuid().ToString();

                })
            };
            var menuItem2 = new MenuItem
            {
                Header = "Copy",
                Command = new Command(() =>
                {
                    Clipboard.SetText(AssociatedObject.Text);
                })
            };

            AssociatedObject.ContextMenu = new ContextMenu { };  
            AssociatedObject.ContextMenu.Items.Add(menuItem);
            AssociatedObject.ContextMenu.Items.Add(menuItem2);
            AssociatedObject.ContextMenu.Items.Add(new Separator());
            AssociatedObject.ContextMenu.Items.Add(new MenuItem { Header = "Cut", Command = ApplicationCommands.Cut });
            AssociatedObject.ContextMenu.Items.Add(new MenuItem { Header = "Copy", Command = ApplicationCommands.Copy });
            AssociatedObject.ContextMenu.Items.Add(new MenuItem { Header = "Paste", Command = ApplicationCommands.Paste });

            base.OnAttached();
        }
    }
}

