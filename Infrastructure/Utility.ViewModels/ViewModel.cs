using Utility.Enums;

namespace Utility.ViewModels
{
    public class ViewModel : NotifyPropertyChangedBase
    {


        public Guid Guid { get; set; }
        public string Name { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public bool IsChecked { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Visible;
        public bool IsEnabled { get; set; } = true;

        public bool IsDeleted { get; set; }

        public bool IsOpen { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public double Area { get; set; } = 1d;

        public double? Width { get; set; }
        public double? Height { get; set; }

        public Orientation Orientation { get; set; }

        public DataPresentationType DataPresentationType { get; set; }

        public int DataPresentationStyle { get; set; }

        public string? DataTemplateKey { get; set; }

        public Arrangement ItemsPanelType { get; set; }


        public Orientation ItemsPanelOrientation { get; set; }

        public int? Rows { get; set; }

        public int? Columns { get; set; }

        public string? ItemsPanelTemplate { get; set; }

    }
}
