using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.WPF.Reactive;

namespace UtilityWpf.Controls.Meta.ViewModels
{
    internal class AssemblyComboBoxViewModelMapper
    {
        public static IDisposable Connect(AssemblyComboBox assemblyComboBox, AssemblyComboBoxViewModel comboBoxViewModel)
        {
            CompositeDisposable composite = new();

            //assemblyComboBox
            //    .WhenAnyValue(a => a.DemoType)
            //    .Subscribe(comboBoxViewModel.demoTypeViewModel)
            //    .DisposeWith(composite);

            //comboBoxViewModel
            //    .demoTypeViewModel
            //    .Subscribe(a => assemblyComboBox.ItemsSource = a.CollectionViewModel.Collection)
            //    .DisposeWith(composite);

            //comboBoxViewModel
            //    .demoTypeViewModel
            //    .Subscribe(a => assemblyComboBox.FilterCollection = a.FilterCollectionViewModel.Collection)
            //    .DisposeWith(composite);

            assemblyComboBox
                .SelectSingleSelectionChanges()
                .CombineLatest(assemblyComboBox.LoadedChanges(), (a, b) => a)
                .DistinctUntilChanged()
                //.CombineLatest(assemblyComboBox.WhenAnyValue(a => a.DemoType))
                .Subscribe(comboBoxViewModel.selectedItemViewModel)
                .DisposeWith(composite);

            comboBoxViewModel
                .selectedItemViewModel
                .Subscribe(a => assemblyComboBox.SelectedItem = a)
                .DisposeWith(composite);

            //assemblyComboBox
            //    .SelectOutputChanges()
            //    .CombineLatest(assemblyComboBox.WhenAnyValue(a => a.DemoType))
            //    .Subscribe(comboBoxViewModel.checkedViewModel)
            //    .DisposeWith(composite);

            return composite;
        }
    }
}