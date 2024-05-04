using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Utility.Collections;

namespace Utility.WPF.Demo.Panels
{
    public class Comparer<T> : IComparer<T>
    {
        private readonly SortDescriptionCollection sortDescriptions;

        public Comparer(SortDescriptionCollection sortDescriptions)
        {
            this.sortDescriptions = sortDescriptions;
        }

        public int Compare(T? x, T? y)
        {
            int diff = 0;
            foreach (SortDescription sd in sortDescriptions)
            {
                IComparable value = x.GetType().GetProperty(sd.PropertyName).GetValue(x) as IComparable;
                diff = value.CompareTo((IComparable)y.GetType().GetProperty(sd.PropertyName).GetValue(y));
                
                if (sd.Direction == ListSortDirection.Ascending ? diff <0 : diff >0)
                {
                    //diff = value.CompareTo((IComparable)items[i].GetType().GetProperty(sd.PropertyName).GetValue(items[i]));
                    break;
                }     
            }

            return diff;
        }
    }

    //class SortYearAscendingHelper : IComparer<Car>
    //{
    //    int IComparer.Compare(object a, object b)
    //    {
    //        Car c1 = (Car)a;
    //        Car c2 = (Car)b;
    //        if (c1.year > c2.year)
    //            return 1;
    //        if (c1.year < c2.year)
    //            return -1;
    //        else
    //            return 0;
    //    }
    //}

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AnimatedListPanelView : UserControl, INotifyPropertyChanged
    {
        SortableObservableCollection<RankedString> collection;
        SortDescriptionCollection sortDescriptions = new();

        public AnimatedListPanelView()
        {
            DataContext = this;

            InitializeComponent();


            collection = new SortableObservableCollection<RankedString>(new Comparer<object>(sortDescriptions))
            {
                new RankedString(1,"test3"),
                new RankedString(3,"test1"),
                new RankedString(3,"test0"),
                new RankedString(1,"test1"),
                new RankedString(2,"test3"),
                new RankedString(3,"test4"),
                new RankedString(3,"test3"),
                new RankedString(2,"test1"),
                new RankedString(1,"test4"),
                new RankedString(1,"test2"),
                new RankedString(2,"test2"),
                new RankedString(2,"test0"),
                new RankedString(3,"test2"),
                new RankedString(2,"test4"),
                new RankedString(1,"test0"),
            };

            IC.ItemsSource = collection;
        }

        private void MoveItemClicked(object sender, RoutedEventArgs e)
        {
            //if(collection.SortDescriptions.Count < 2)
            //{
            //    collection.SortDescriptions.Add(new SortDescription("Rank", ListSortDirection.Descending));
            //}
        }

        

        private void AddItemClicked(object sender, RoutedEventArgs e)
        {
            if (TB1.Text != "")
            {
                string text = TB1.Text;

                if (int.TryParse(text.Substring(text.Length - 1), out int res))
                {
                    collection.Add(new RankedString(1, TB1.Text));
                    TB1.Clear();
                }
            }
        }


        private void FilterClicked(object sender, RoutedEventArgs e)
        {
            //if (collection.Filter == null)
            //    collection.Filter = (x => x.Content.Length > 0 ? int.Parse(x.Content.Substring(x.Content.Length - 1)) > 2 : false);
            //else
            //    collection.Filter = null;
        }



        private void DeleteItemClicked(object sender, RoutedEventArgs e)
        {
            if (collection.Count > 0)
                collection.RemoveAt(0);
        }

        private void SetItemClicked(object sender, RoutedEventArgs e)
        {
            if (collection.Count > 0)
            {
                collection[0].Content = TB2.Text;
                TB2.Clear();
            }
        }

        private void ClearItemsClicked(object sender, RoutedEventArgs e)
        {
            collection.Clear();
        }

        private void SortItemsClicked(object sender, RoutedEventArgs e)
        {
             switch (sortDescriptions.Count)
            {
                case 0:
                    sortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
                    break;
                case 1:
                    sortDescriptions.Add(new SortDescription("Rank", ListSortDirection.Ascending));
                    break;
                default:
                    sortDescriptions.Clear();
                    break;
            }

            collection.Sort();
        }

        private void BreakpointClicked(object sender, RoutedEventArgs e)
        {
            var temp = collection;
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }

    [DebuggerDisplay("{Rank} : {Content}")]
    public class RankedString : INotifyPropertyChanged
    {
        public string RankedContent { get { return $"{Rank} : {Content}"; } }

        private int _rank;
        public int Rank
        {
            get { return _rank; }
            set { _rank = value; OnPropertyChanged("Rank"); OnPropertyChanged("RankedContent"); }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; OnPropertyChanged("Content"); OnPropertyChanged("RankedContent"); }
        }

        private NonComparable _nonComparable;
        public NonComparable NC
        {
            get { return _nonComparable; }
            set { _nonComparable = value; OnPropertyChanged("NC"); }
        }

        public RankedString(int rank, string content)
        {
            Rank = rank;
            Content = content;
        }

        public override string ToString()
        {
            return RankedContent;
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public class NonComparable { }
    }


}
