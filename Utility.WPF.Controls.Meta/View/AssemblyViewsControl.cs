using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Controls.Meta
{
    public class AssemblyViewsControl : ContentControl
    {
        public AssemblyViewsControl()
        {
            var dockPanel = new DockPanel();

            foreach (var child in CreateChildren())
            {
                dockPanel.Children.Add(child);
            }

            Content = dockPanel;
        }

        private static IEnumerable<Control> CreateChildren()
        {
            //var viewModel = new ViewModel();
            //var b = CreateDualButtonControl(viewModel.DemoType);
            //Connector.Connect(b, viewModel);
            //yield return b;

            var c = CreateAssemblyComboBox();
            //Connector.Connect(c, viewModel);
            yield return c;

            var v = CreateViewsMasterDetailControl(CreateBinding(c));
            //Connector.Connect(v, viewModel);
            yield return v;


            //static DualButtonControl CreateDualButtonControl(AssemblyType demoType)
            //{
            //    var dualButtonControl = new DualButtonControl
            //    {
            //        Main = AssemblyType.UserControl,
            //        Alternate = AssemblyType.ResourceDictionary,
            //        Orientation = Orientation.Horizontal,
            //        Value = DualButtonControl.KeyToValue(demoType, AssemblyType.UserControl, AssemblyType.ResourceDictionary)
            //    };
            //    DockPanel.SetDock(dualButtonControl, Dock.Top);
            //    return dualButtonControl;
            //}

            static AssemblyComboBox CreateAssemblyComboBox()
            {
                var comboBox = new AssemblyComboBox();
                DockPanel.SetDock(comboBox, Dock.Top);
                return comboBox;
            }

            static ViewsMasterDetail CreateViewsMasterDetailControl(Binding binding)
            {
                var viewsDetailControl = new ViewsMasterDetail { };
                BindingOperations.SetBinding(
                    viewsDetailControl,
                    ViewsMasterDetail.AssemblyProperty,
                    binding);
                return viewsDetailControl;
            }

            static Binding CreateBinding(AssemblyComboBox comboBox)
            {
                return new()
                {
                    Path = new PropertyPath(nameof(ComboBox.SelectedValue)),
                    Source = comboBox
                };
            }
        }

      //  class Connector
       // {
            //public static void Connect(DualButtonControl dualButtonControl/*, ViewModel viewModel*/)
            //{
            //    dualButtonControl
            //        .Toggles()
            //        .Select(size => Enum.Parse<AssemblyType>(size.Key.ToString()))
            //        .Subscribe(demoType => viewModel.DemoType = demoType);
            //}

            //public static void Connect(AssemblyComboBox comboBox, ViewModel viewModel)
            //{
            //    viewModel
            //        .WhenAnyValue(a => a.DemoType)
            //        .Subscribe(a => comboBox.DemoType = a);
            //}

            //public static void Connect(ViewsMasterDetail detailControl, ViewModel viewModel)
            //{
            //    viewModel
            //        .WhenAnyValue(a => a.DemoType)
            //        .Subscribe(a => detailControl.DemoType = a);
            //}
      //  }

        //class ViewModel : ReactiveObject
        //{
        //    private DualButtonEntity? first;
        //    private bool isInitialised = false;

        //    public AssemblyType DemoType
        //    {
        //        get
        //        {
        //            if (!isInitialised)
        //            {
        //                FreeSqlFactory.InitialiseSQLite();
        //                isInitialised = true;
        //            }
        //            first ??= DualButtonEntity.Select.First();

        //            if (first == null)
        //            {
        //                first = new DualButtonEntity { DemoType = AssemblyType.UserControl };
        //                first.InsertAsync();
        //            }

        //            return first.DemoType;
        //        }
        //        set
        //        {
        //            (first ??= DualButtonEntity.Select.First()).DemoType = value;
        //            first.UpdateAsync();
        //            this.RaisePropertyChanged();
        //        }
        //    }
        //    class DualButtonEntity : BaseEntity<DualButtonEntity, Guid>
        //    {
        //        public AssemblyType DemoType { get; set; }
        //    }
        //}
    }
}