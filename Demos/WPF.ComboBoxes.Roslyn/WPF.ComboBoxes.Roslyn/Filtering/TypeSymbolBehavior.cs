using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Xaml.Behaviors;

namespace WPF.ComboBoxes.Roslyn
{
    internal class TypeSymbolBehavior : Behavior<ComboBox>
    {
        ObservableCollection<INamedTypeSymbol> collection;

        protected override void OnAttached()
        {           
            AssociatedObject.DropDownOpened += (s, e) =>
            {
                if (FilteringBehavior.GetSource(AssociatedObject) is null)
                {
                    collection = new ObservableCollection<INamedTypeSymbol>( 
                        App.Compilation.GetValidTypes());
                    FilteringBehavior.SetSource(AssociatedObject, collection);
                }
            };
            base.OnAttached();
  
        }
    }


    internal class MethodSymbolBehavior : Behavior<ComboBox>
    {
        ObservableCollection<IMethodSymbol> collection;

        protected override void OnAttached()
        {           
            AssociatedObject.DropDownOpened += (s, e) =>
            {
                if (FilteringBehavior.GetSource(AssociatedObject) is null)
                {
                    collection = new ObservableCollection<IMethodSymbol>(
                          App.Compilation.GetValidTypes()
                                .Where(t => t.IsPublic())
                                .SelectMany(t => t.GetMethods()));

                    FilteringBehavior.SetSource(AssociatedObject, collection);
                }
            };
            base.OnAttached();
   
        }


     
    }
}
