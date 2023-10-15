using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Demo.Infrastructure;
using Svc = Utility.Trees.Demo.Infrastructure.Service;

namespace Utility.Trees.Demo.Two
{

    public class Converter
    {
        private readonly ConnectionsView userControl;
        private ConnectionsViewModel dataContext;
        private Control hitResultsList;
        private Control hitResultsList2;

        public Converter(ConnectionsView userControl)
        {
            this.userControl = userControl;

        }

        public LineViewModel To(ConnectionModel connectionViewModel)
        {
            dataContext = userControl.DataContext as ConnectionsViewModel;
            Point pointA, pointB;
            Tree viewmodel = TreeHelper2.Match(dataContext.ViewModel, new((a) => (a.Data as IGuid).Guid == connectionViewModel.Guid)) as Tree;

            var treeViewItem = TreeHelper.FindRecursive<TreeViewItem>(userControl.TreeView, viewmodel);
            {
                var y = userControl.TreeView.FindDepth(treeViewItem);
                pointA = new Point(userControl.TreeView.ActualWidth, y);
            }
            IName service = null;
            foreach (var x in dataContext.ServiceModel)
            {
                var data = x as IName;
                if (connectionViewModel.ServiceGuid == (x as IGuid).Guid)
                    service = data;
            }

            var listBoxItem = TreeHelper.FindRecursive<ListBoxItem>(userControl.ListBox, service);
            {

                Size size = listBoxItem.RenderSize;
                Point ofs = new(0, size.Height / 2d);
                pointB = listBoxItem.TransformToAncestor(userControl).Transform(ofs);

            }

            return new LineViewModel { Guid = connectionViewModel.Guid, StartPoint = pointA, EndPoint = pointB };
        }

        public ConnectionModel To(LineViewModel lineViewModel)
        {
            dataContext = userControl.DataContext as ConnectionsViewModel;
            //HitTestResult result = VisualTreeHelper.HitTest(userControl, lineViewModel.StartPoint);
            VisualTreeHelper.HitTest(userControl, new HitTestFilterCallback(MyHitTestFilter), new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(lineViewModel.StartPoint));
            if (hitResultsList == null)
            {
                MessageBox.Show($"{nameof(hitResultsList)} equals null");
                return null;
            }
            var persist = (hitResultsList.DataContext as Tree).Data as IGuid;
            VisualTreeHelper.HitTest(userControl.ListBox, new HitTestFilterCallback(MyHitTestFilter2), new HitTestResultCallback(MyHitTestResult2), new PointHitTestParameters(new Point(10, lineViewModel.EndPoint.Y)));
            var service = hitResultsList2.DataContext as Svc;
            return new ConnectionModel { Guid = lineViewModel.Guid, ServiceGuid = service.Guid, ViewModelGuid = persist.Guid };
        }

        // Filter the hit test values for each object in the enumeration.
        public HitTestFilterBehavior MyHitTestFilter(DependencyObject o)
        {
            // Test for the object value you want to filter.
            if (o is TreeViewItem treeViewItem)
            {
                hitResultsList = treeViewItem;
                return HitTestFilterBehavior.Continue;
            }
            else
            {
                // Visual object is part of hit test results enumeration.
                return HitTestFilterBehavior.Continue;
            }
        }

        // Return the result of the hit test to the callback.
        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            if (result.VisualHit is TreeViewItem treeViewItem)
            {
                hitResultsList = treeViewItem;
                return HitTestResultBehavior.Continue;
            }
            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }

        public HitTestFilterBehavior MyHitTestFilter2(DependencyObject o)
        {
            // Test for the object value you want to filter.
            if (o is ListBoxItem treeViewItem)
            {
                hitResultsList2 = treeViewItem;
                return HitTestFilterBehavior.Stop;
            }
            else
            {
                // Visual object is part of hit test results enumeration.
                return HitTestFilterBehavior.Continue;
            }
        }

        // Return the result of the hit test to the callback.
        public HitTestResultBehavior MyHitTestResult2(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            if (result.VisualHit is ListBoxItem treeViewItem)
            {
                hitResultsList2 = treeViewItem;
                return HitTestResultBehavior.Stop;
            }
            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }
    }

}