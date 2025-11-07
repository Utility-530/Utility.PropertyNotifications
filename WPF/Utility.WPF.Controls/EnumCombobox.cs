using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;
using Utility.WPF.Behaviors;

namespace Utility.WPF.Controls
{
    public class EnumComboBox : ComboBox
    {
        static EnumComboBox()
        {
        }

        public EnumComboBox()
        {
            //EnumSelectorBehavior behavior = new();
            //BindingOperations.SetBinding(behavior, EnumSelectorBehavior.SelectedEnumProperty, new Binding()
            //{
            //    Path = new PropertyPath("SelectedEnum"),
            //    Mode = BindingMode.TwoWay,
            //    Source = this
            //});
            //Interaction.GetBehaviors(this).Add(behavior);

            EnumComboBehavior behavior = new();
            BindingOperations.SetBinding(behavior, EnumComboBehavior.ValueProperty, new Binding()
            {
                Path = new PropertyPath(nameof(EnumComboBox.SelectedItem)),
                Mode = BindingMode.TwoWay,
                Source = this
            });
            Interaction.GetBehaviors(this).Add(behavior);
        }

        //public static readonly DependencyProperty SelectedEnumProperty =
        //DependencyProperty.Register("SelectedEnum", typeof(Enum), typeof(EnumComboBox), new FrameworkPropertyMetadata
        //{
        //    BindsTwoWayByDefault = true,
        //    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        //});

        //public Enum SelectedEnum
        //{
        //    get => (Enum)GetValue(SelectedEnumProperty);
        //    set => SetValue(SelectedEnumProperty, value);
        //}
    }
}