using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Utility.WPF.Behaviors
{
    public class ResetComboBoxOnOpenedBehavior : Behavior<ComboBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.DropDownOpened += ComboBox_DropDownOpened;
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox combobox = sender as ComboBox;

            if (combobox.SelectedIndex != -1)
            {
                combobox.SelectedIndex = -1;
                combobox.SelectedValue = null;
                combobox.SelectedItem = null;

            }
        }

    }



}
