using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Utility.WPF
{

    public class EditRoutedEventArgs : RoutedEventArgs
        {
            public EditRoutedEventArgs(bool isAccepted, object newObject, RoutedEvent routedEvent, object source) : base(routedEvent, source)
            {
                Edit = newObject;
                IsAccepted = isAccepted;
            }

            public object Edit { get; }
            public bool IsAccepted { get; }
        }

        public delegate void FinishEditRoutedEventHandler(object sender, EditRoutedEventArgs e);

}
