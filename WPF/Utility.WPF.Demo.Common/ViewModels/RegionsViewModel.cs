using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Utility.Models;

namespace Utility.WPF.Demo.Common.ViewModels
{
    public class RegionsViewModel : BaseViewModel
    {
        private Region _selectedRegion;
        private ObservableCollection<Region> _regions = new();

        public RegionsViewModel()
        {
        }

        public Region SelectedRegion
        {
            get => _selectedRegion;
            set => this.Set(ref _selectedRegion, value);
        }

        public ObservableCollection<Region> Regions => _regions;
     

        public void AddRegion1()
        {
            Regions.Add(new Region("Region 1: Australia and New Zealand", 10, 10, 100000000, 20000000));
        }

        public void AddRegion2()
        {
            Regions.Add(new Region("Region 2: UK", 100, 100, 200, 300));
        }

        public void RemoveRegion()
        {
            if (SelectedRegion != null)
            {
                int index = Regions.IndexOf(SelectedRegion);
                Regions.Remove(SelectedRegion);
                if (index > 0)
                {
                    --index;
                }
                if (Regions.Count > 0)
                {
                    SelectedRegion = Regions[index];
                }
                else
                {
                    SelectedRegion = null;
                }
            }
        }

        public void ClearRegions()
        {
            Regions.Clear();
        }
    }

    public class Region
    {
        public Region(string text, int left, int top, int right, int bottom)
        {
            Text = text;
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Region() { }

        public string Text { get; set; }

        public Type Type => typeof(int);

        public Orientation Orientation { get; set; }

        public int Left { get; set; }

        [Browsable(false)]
        public int Top { get; set; }

        [DisplayName("aasd")]
        public int Right { get; set; }

        [DisplayName("")]
        public int Bottom { get; set; }
    }
}
