using System.Windows;
using Utility.WPF.Controls.Lists;
using Utility.WPF.Controls.Meta.ViewModels;

namespace Utility.WPF.Controls.Meta
{
    public class TypesComboBox : CheckBoxesComboControl
    {
        private TypeComboBoxService viewModel;

        public TypesComboBox()
        {
            //SelectedIndex = 0;
            FontWeight = FontWeights.DemiBold;
            //FontSize = 14;
            Margin = new Thickness(4);
            //Width = (this.Parent as FrameworkElement)?.ActualWidth ?? 1000;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Height = 80;
            KeyPath = nameof(ViewModelEntity.Key);
            IsCheckedPath = nameof(ViewModelEntity.IsChecked);
            IsSelectedPath = nameof(ViewModelEntity.IsSelected);
            SelectedValuePath = nameof(ViewModelEntity.Value);

            viewModel = new TypeComboBoxService();
            var dis = ComboBoxViewModelMapper.Connect(this, viewModel);
            this.Unloaded += (s, e) => dis.Dispose();
        }

        public void Deselect()
        {
            viewModel.Deselect();
        }
    }
}