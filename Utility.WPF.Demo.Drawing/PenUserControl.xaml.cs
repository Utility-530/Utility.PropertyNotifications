using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility.WPF.Demo.Drawing
{
    /// <summary>
    /// Interaction logic for PenUserControl.xaml
    /// </summary>
    public partial class PenUserControl : UserControl
    {
        Point point0 = new(0, 0), point1 = new(0, 0);

        public PenUserControl()
        {
            InitializeComponent();
            //this.LayoutUpdated += ConnectionsUserControl_LayoutUpdated;
            Spinner1.ValueChanged += Spinner1_ValueChanged;
            Spinner2.ValueChanged += Spinner2_ValueChanged; ;
        }

        private void Spinner1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            point0 = new Point((double)e.NewValue, (double)e.NewValue);
            InvalidateVisual();
        }

        private void Spinner2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            point1 = new Point(this.ActualWidth-(double)e.NewValue, this.ActualHeight- (double)e.NewValue);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawLine(Pen, point0, point1);
        }

        private void ConnectionsUserControl_LayoutUpdated(object? sender, EventArgs e)
        {
            //Size size = RenderSize;
            //Point ofs = new Point(size.Width / 2, size.Height);
            //var point = TransformToVisual(Grid).Transform(ofs);
        }


        protected Pen Pen
        {
            get
            {

                var ResourcePen = (Pen)this.TryFindResource("ChildConnectionPen");
                // Make a copy of the resource pen so it can
                // be modified, the resource pen is frozen.
                Pen connectorPen = ResourcePen.Clone();

                // Set opacity based on the filtered state.
                //connectorPen.Brush.Opacity = isFiltered ? Const.OpacityFiltered : Const.OpacityNormal;

                //// Create animation if the filtered state has changed.
                //if (animation != null)
                //    connectorPen.Brush.BeginAnimation(Brush.OpacityProperty, animation);

                return connectorPen;
            }
        }

    }
}
