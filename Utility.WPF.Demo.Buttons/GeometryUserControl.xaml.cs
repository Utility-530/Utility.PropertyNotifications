using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Utility.WPF.Controls.Buttons;

namespace Utility.WPF.Demo.Buttons
{
    /// <summary>
    /// Interaction logic for GeometryUserControl.xaml
    /// </summary>
    public partial class GeometryUserControl : UserControl
    {
        private readonly TypeConverter converter;

        public GeometryUserControl()
        {
            InitializeComponent();

            this.PathTextBox.Text = GeometryButton.InitialData;
            converter = TypeDescriptor.GetConverter(typeof(Geometry));
            this.PathTextBox.TextChanged += PathTextBox_TextChanged;
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ErrorTextBlock.Text = string.Empty;
            try
            {
                PathButton.Data = converter.ConvertFrom(this.PathTextBox.Text) as Geometry ?? throw new Exception("fgdgd 443 3");
            }
            catch (Exception ex)
            {
                this.ErrorTextBlock.Text = ex.Message;
            }
        }
    }
}