//using Microsoft.Xaml.Behaviors;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
//using System.Windows.Data;
//using Utility.Descriptors;
//using Utility.Interfaces.NonGeneric;
//using Utility.Trees.Abstractions;
//using Utility.WPF.Controls.Trees;

//namespace Utility.Trees.Demo.Filters
//{
//    public class RefreshTargetAction : TriggerAction<FrameworkElement>
//    {

//        protected override void Invoke(object parameter)
//        {
//            if(parameter is RoutedEventArgs {  Source : CustomTreeViewItem source})
//            {
//                BindingOperations.GetBindingExpressionBase(source, CustomTreeViewItem.NodeItemsSourceProperty).UpdateTarget();
//            }
//        }
//    }

//}
