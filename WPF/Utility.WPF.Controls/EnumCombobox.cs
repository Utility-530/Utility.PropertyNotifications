using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Attached;
using Utility.Enums;
using Utility.WPF.Controls.Base;
using Evan.Wpf;
using Jellyfish;
using Utility.Helpers;
using System.Windows.Input;
using System.Reactive.Subjects;
using MintPlayer.ObservableCollection;
using Utility.WPF.Helpers;
using Utility.Models;
using Utility.Infrastructure;
using Visibility = System.Windows.Visibility;
using Arrangement = Utility.Enums.Arrangement;
using System.Windows.Data;
using Utility.WPF.Behaviors;
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