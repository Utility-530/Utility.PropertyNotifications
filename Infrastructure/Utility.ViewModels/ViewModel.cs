using Utility.Enums;

namespace Utility.ViewModels
{
    public class ViewModel : NotifyPropertyChangedBase
    {
        private bool booleanValue;
        private object referenceValue;
        #region values

        public string StringValue { get; set; }

        public bool BooleanValue
        {
            get => booleanValue; set
            {
                booleanValue = value;
                RaisePropertyChanged();

            }
        }
        public double NumberValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public int IntegerValue { get; set; }

        public object ReferenceValue
        {
            get
            {
                return referenceValue;
            }

            set
            {
                referenceValue = value;
                RaisePropertyChanged();
            }
        }


        #endregion values


        public Guid Guid { get; set; }
        public Guid ParentGuid { get; set; }
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

        public Orientation? Orientation { get; set; }

        public DataPresentationType DataPresentationType { get; set; }

        public int DataPresentationStyle { get; set; }

        public string? DataTemplateKey { get; set; }

        public Arrangement ItemsPanelType { get; set; }


        public Orientation? ItemsPanelOrientation { get; set; }

        public int? Rows { get; set; }

        public int? Columns { get; set; }

        public string? ItemsPanelTemplate { get; set; }

    }
}
