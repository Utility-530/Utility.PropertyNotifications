using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Utility.WPF
{

        public class NewObjectRoutedEventArgs : RoutedEventArgs
        {
            public NewObjectRoutedEventArgs(bool isAccepted, object newObject, RoutedEvent routedEvent, object source) : base(routedEvent, source)
            {
                NewObject = newObject;
                IsAccepted = isAccepted;
            }

            public object NewObject { get; }
            public bool IsAccepted { get; }
        }

        public delegate void FinishEditRoutedEventHandler(object sender, NewObjectRoutedEventArgs e);


    
}
