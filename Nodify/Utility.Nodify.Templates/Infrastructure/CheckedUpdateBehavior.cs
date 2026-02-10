using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design.Behavior;
using Microsoft.Xaml.Behaviors;
using Utility.Nodes;
using Utility.PatternMatchings;
using Utility.WPF.ComboBoxes.Roslyn.Infrastructure;

namespace Utility.Nodify.Views.Infrastructure
{
    public class CheckedUpdateBehavior : Behavior<ComboBox>
    {
        protected override void OnAttached()
        {
            CheckedEvents.AddCheckedChangesHandler(AssociatedObject, handler);
            base.OnAttached();
        }

        private void handler(object sender, CheckedChangesEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: PendingConnectorViewModel viewModel } fe)
            {
                if (AssociatedObject.ItemsSource is IEnumerable<Result> results)
                {
                    foreach (Result result in results)
                    {

                        if (result.Symbol.FullName == e.Name)
                        {
                            if (e.IsChecked)
                            {
                                var additions = new List<PropertyInfo> { };
                                additions.Add(result.Symbol.Item as PropertyInfo);
                                viewModel.ChangeConnectorsCommand.Execute(new CollectionChanges(additions, Array.Empty<PropertyInfo>()));
                            }
                            else
                            {
                                var removals = new List<PropertyInfo> { };
                                removals.Add(result.Symbol.Item as PropertyInfo);
                                viewModel.ChangeConnectorsCommand.Execute(new CollectionChanges(Array.Empty<PropertyInfo>(), removals));
                            }
                        }
                    }
                }
            }
        }
    }
}
