using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace Utility.WPF.Demo.External
{
    /// <summary>
    /// Interaction logic for TextUserControl.xaml
    /// </summary>
    public partial class TextUserControl : UserControl
    {
        public TextUserControl()
        {
            InitializeComponent();
        }
    }

    public partial class TextViewModel : IDataErrorInfo
    {
        private decimal _number = 1.23M;

        public decimal Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }

        private List<string> _stringFormats;

        public List<string> StringFormats
        {
            get
            {
                if (_stringFormats == null)
                {
                    _stringFormats = new List<string>() { "C", "E", "F", "G", "N", "P", "{0:C1}", "{0:C0}" };
                }

                return _stringFormats;
            }
            set
            {
                _stringFormats = value;
            }
        }

        public string Error
        {
            get { throw new System.NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Number" &&
                    (_number < 0 || _number > 10))
                {
                    return "Number must be between zero and ten.";
                }
                else
                {
                    return null;
                }
            }
        }
    }
}