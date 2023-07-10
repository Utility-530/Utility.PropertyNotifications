using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Windows;
using Utility.WPF.Controls.Buttons;
using Utility.Enums;
using System.Reflection;
using Utility.WPF.Helpers;
using Utility.Common;
using ReactiveUI;

namespace Utility.WPF.Controls.Meta
{
    public class AssemblyViewControl : ContentControl
    {
        public AssemblyViewControl(Assembly assembly)
        {
            var dockPanel = new DockPanel();

            foreach (var child in CreateChildren(assembly))
            {
                dockPanel.Children.Add(child);
            }

            Content = dockPanel;


            this.Resources.Add(new DataTemplateKey(
                typeof(KeyValue)),
                TemplateGenerator.CreateDataTemplate(() =>
                {
                    var textBlock = new TextBlock
                    {
                        MinHeight = 20,
                        FontSize = 14,
                        MinWidth = 100
                    };
                    Binding binding = new()
                    {
                        Path = new PropertyPath(nameof(KeyValue.Key)),
                    };
                    textBlock.SetBinding(TextBlock.TextProperty, binding);
                    return textBlock;
                }));
        }

        private static IEnumerable<Control> CreateChildren(Assembly assembly)
        {
            var dualButtonControl = CreateDualButtonControl(AssemblyType.UserControl);
            yield return dualButtonControl;

            var detailControl = CreateViewsMasterDetailControl(assembly, dualButtonControl);
            yield return detailControl;


            static DualButtonControl CreateDualButtonControl(AssemblyType demoType)
            {
                var dualButtonControl = new DualButtonControl
                {
                    Main = AssemblyType.UserControl,
                    Alternate = AssemblyType.ResourceDictionary,
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    Value = SwitchControl.KeyToValue(demoType, AssemblyType.UserControl, AssemblyType.ResourceDictionary)
                };
                DockPanel.SetDock(dualButtonControl, Dock.Top);
                return dualButtonControl;
            }

            static ViewsMasterDetail CreateViewsMasterDetailControl(Assembly assembly, DualButtonControl comboBox)
            {
                var viewsDetailControl = new ViewsMasterDetail { Assembly = assembly };
                comboBox
                    .WhenAnyValue(ass => ass.Value)      
                    .Subscribe(type =>
                    {
                        viewsDetailControl.AssemblyType = (AssemblyType)comboBox.ValueToKey();
                    });
                return viewsDetailControl;
            }
        }


    }
}
