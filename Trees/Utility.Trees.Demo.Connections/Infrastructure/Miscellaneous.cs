using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using Utility.PropertyNotifications;

namespace Utility.Trees.Demo.Connections
{
    public record ConnectionsViewModel : NotifyProperty
    {
        private Point? point0, point1;
        private LineViewModel last;

        public ObservableCollection<LineViewModel> Lines { get; } = new();

        public ObservableCollection<ConnectionModel> Connections { get; } = new();
        public ObservableCollection<Log> Logs { get; } = new();
        public Tree Tree { get; set; }
        public List<Service> ServiceModel { get; set; }

        public Point? Point0
        {
            get => point0;
            set
            {
                point0 = value;
                this.RaisePropertyChanged(ref point0, value);
            }
        }

        public Point? Point1
        {
            get => point1;
            set
            {
                point1 = value;
                this.RaisePropertyChanged(ref point1, value);
            }
        }

        public LineViewModel Last
        {
            get => last;
            set
            {
                last = value;
                this.RaisePropertyChanged(ref last, value);
            }
        }
    }

    public class LineViewModelInfo
    {
        public bool IsPersisted { get; set; }
    }

    public record LineViewModel : NotifyProperty
    {
        public LineViewModel()
        {

        }
        private Point _startPoint;

        public Guid Id { get; set; }

        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                this.RaisePropertyChanged(ref _startPoint, value);
            }
        }

        private Point _endPoint;
        private bool isSelected;

        public Point EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint = value;
                this.RaisePropertyChanged(ref _endPoint, value);
            }
        }

        public Direction Direction { get; set; }
        public bool IsSelected
        {
            get => isSelected; set
            {
                if (isSelected == value) return;
                isSelected = value;
                RaisePropertyChanged(ref isSelected, value);
            }
        }
    }

    public record Log(string MethodName, DateTime DateTime)
    {

    }


    public record ConnectionModel : NotifyProperty
    {
        private bool isSelected;
        private bool isDeleted;

        public Guid Id { get; set; }

        public string ViewModelName { get; set; }

        public string ServiceName { get; set; }
        public Movement Movement { get; set; }
        public string ParameterName { get; set; }

        public bool IsSelected
        {
            get => isSelected; set
            {
                if (isSelected == value) return;

                isSelected = value;
                RaisePropertyChanged(ref isSelected, value);
            }
        }

        public bool IsDeleted
        {
            get => isDeleted; set
            {
                isDeleted = value;
                RaisePropertyChanged(ref isDeleted, value);
            }
        }
    }



    public static class ClearTreeSelection
    {
        public static void ClearSelections(this ItemsControl tview)
        {

            ClearTreeViewItemsControlSelection(tview.Items, tview.ItemContainerGenerator);

            static void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
            {
                for (int i = 0; i < ic.Count; i++)
                {
                    // Get the TreeViewItem
                    if (icg.ContainerFromIndex(i) is TreeViewItem tvi)
                    {
                        //Recursive call to traverse deeper levels
                        ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
                        //Deselect the TreeViewItem 
                        tvi.IsSelected = false;
                    }
                }
            }
        }

        public static Point FindLocation(this ItemsControl treeView, Control treeViewItem)
        {
            var headerControl = GetHeaderControl(treeViewItem);
            Point pointA;

            if (headerControl == null)
            {
                pointA = new(treeViewItem.ActualWidth, treeViewItem.RenderSize.Height / 2d);
            }
            else
            {
                var x = headerControl.RenderSize;
                Point ofs = new(headerControl.ActualWidth, x.Height / 2d);
                pointA = headerControl.TransformToAncestor(treeViewItem).Transform(ofs);

            }

            return treeViewItem.TransformToAncestor(treeView).Transform(pointA);
        }

        //public static double FindDistanceFromTop(this ItemsControl treeView, Control treeViewItem)
        //{
        //    var headerControl = GetHeaderControl(treeViewItem);
        //    Point pointA;

        //    if (headerControl == null)
        //    {
        //        pointA = new(0, treeViewItem.RenderSize.Height / 2d);
        //    }
        //    else
        //    {
        //        var x = headerControl.RenderSize;
        //        Point ofs = new(0, x.Height / 2d);
        //        pointA = headerControl.TransformToAncestor(treeViewItem).Transform(ofs);

        //    }

        //    return treeViewItem.TransformToAncestor(treeView).Transform(pointA).Y;
        //}

        public static FrameworkElement? GetHeaderControl(Control item)
        {
            return (FrameworkElement?)item?.Template.FindName("PART_Header", item);
        }
    }

    public enum Direction
    {
        StartToEnd, EndToStart
    }
    public enum Movement
    {
        FromViewModelToService, ToViewModelFromService
    }
}
