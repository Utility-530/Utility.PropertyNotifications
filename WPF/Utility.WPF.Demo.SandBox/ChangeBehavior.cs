using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Input;
using Utility.WPF.Controls.Objects;

namespace Utility.WPF.Demo.SandBox
{
    public class ChangeBehavior:Behavior<JsonControl>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.MouseLeave += AssociatedObje2ct_IsVisibleChanged;

            base.OnAttached();
        }

        private void AssociatedObje2ct_IsVisibleChanged(object sender, MouseEventArgs e)
        {
            BindingExpression bindingExpression = AssociatedObject.GetBindingExpression(JsonControl.ObjectProperty);
            // Force ConvertBack to be called
            bindingExpression?.UpdateSource();
        }
    }
}
