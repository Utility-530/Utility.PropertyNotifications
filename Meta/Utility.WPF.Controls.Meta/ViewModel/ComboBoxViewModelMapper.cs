using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.WPF.Controls.Lists;
using Utility.WPF.Reactives;

namespace Utility.WPF.Controls.Meta.ViewModels
{
    public class ComboBoxViewModelMapper
    {
        public static IDisposable Connect(CheckBoxesComboControl comboBox, ComboBoxServcice service)
        {
            CompositeDisposable composite = new();

            //comboBox
            //    .WhenAnyValue(a => a.DemoType)
            //    .Subscribe(comboBoxViewModel.demoTypeViewModel)
            //    .DisposeWith(composite);

            service
                .demoTypeViewModel
                .Subscribe(a => comboBox.ItemsSource = a.CollectionViewModel.Children)
                .DisposeWith(composite);

            service
                .demoTypeViewModel
                .Subscribe(a => comboBox.FilterCollection = a.FilterCollectionViewModel.Collection)
                .DisposeWith(composite);

            comboBox
                .Changes()
                .CombineLatest(comboBox.LoadedChanges(), (a, b) => a)
                .DistinctUntilChanged()
                //.CombineLatest(assemblyComboBox.WhenAnyValue(a => a.DemoType))
                .Subscribe(service.selectedItemViewModel)
                .DisposeWith(composite);

            service
                .selectedItemViewModel
                .Subscribe(a => comboBox.SelectedItem = a)
                .DisposeWith(composite);

            //comboBox
            //    .SelectOutputChanges()
            //    .CombineLatest(comboBox.WhenAnyValue(a => a.DemoType))
            //    .Subscribe(comboBoxViewModel.checkedViewModel)
            //    .DisposeWith(composite);

            return composite;
        }
    }
}