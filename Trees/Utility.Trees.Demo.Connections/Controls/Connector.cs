using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Utility.Trees.Demo.Connections
{
    public class Connector :HeaderedContentControl
    {


        static Connector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Connector), new FrameworkPropertyMetadata(typeof(Connector)));

        }


        public Connector()
        {
            this.MouseDown += Connector_MouseDown;
        }

        private void Connector_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((UIElement)sender);



            //Point ofs = new(0, size.Height / 2d);
            //var pointA = TreeView2.TransformToAncestor(Container).Transform(ofs);
            //var pointB = element.TransformToAncestor(TreeView2).Transform(ofs);
            //point1 = new Point(pointA.X /*+ pointB.X*/, pointB.Y);
            //if (point0.HasValue)
            //{
            //    point0 = point1 = null;
            //    last = null;
            //}
            //else
            //    Add(Direction.EndToStart);
        }


    }
}
