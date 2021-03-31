using Utility.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections;

namespace Utility.Tasks.View
{
    public class AsyncWorkerQueueControl : Control 
    {
        public static readonly DependencyProperty ReadyItemsProperty = DependencyProperty.Register("ReadyItems", typeof(IEnumerable),
            typeof(AsyncWorkerQueueControl), new PropertyMetadata(null, Changed2));    
        
        public static readonly DependencyProperty CreatedItemsProperty = DependencyProperty.Register("CreatedItems", typeof(IEnumerable),
            typeof(AsyncWorkerQueueControl), new PropertyMetadata(null, Changed2));

        public static readonly DependencyProperty RunningItemsProperty = DependencyProperty.Register("RunningItems", typeof(IEnumerable), 
            typeof(AsyncWorkerQueueControl),new PropertyMetadata(null, Changed));      
               
        public static readonly DependencyProperty TerminatedItemsProperty = DependencyProperty.Register("TerminatedItems", typeof(IEnumerable), 
            typeof(AsyncWorkerQueueControl),new PropertyMetadata(null, Changed));   
        
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), 
            typeof(AsyncWorkerQueueControl),new PropertyMetadata(null, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }       

        private static void Changed2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public IEnumerable RunningItems
        {
            get { return (IEnumerable)GetValue(RunningItemsProperty); }
            set { SetValue(RunningItemsProperty, value); }
        }

        public IEnumerable ReadyItems
        {
            get { return (IEnumerable)GetValue(ReadyItemsProperty); }
            set { SetValue(ReadyItemsProperty, value); }
        }

        public IEnumerable CreatedItems
        {
            get { return (IEnumerable)GetValue(CreatedItemsProperty); }
            set { SetValue(CreatedItemsProperty, value); }
        }

        public IEnumerable TerminatedItems
        {
            get { return (IEnumerable)GetValue(TerminatedItemsProperty); }
            set { SetValue(TerminatedItemsProperty, value); }
        }
        
        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }


        public AsyncWorkerQueueControl()
        {
        }
    }
}
