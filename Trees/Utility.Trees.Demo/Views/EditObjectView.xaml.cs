using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Utility.Common.Model;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Filters;
using Utility.Persists;
using Utility.ProjectStructure;
using Utility.Trees.Demo.Infrastructure;
using Utility.WPF.Controls.Meta;
using Utility.WPF.Controls.Meta.ViewModels;
using Utility.WPF.Meta;
using Utility.WPF.Reactives;

namespace Utility.Trees.Demo
{
    //public class ObjectListBox:Control
    //{
    //    public static readonly DependencyProperty ObjectProperty =
    //        DependencyProperty.Register("Object", typeof(object), typeof(ObjectListBox), new PropertyMetadata());


    //    public object Object
    //    {
    //        get { return (object)GetValue(ObjectProperty); }
    //        set { SetValue(ObjectProperty, value); }
    //    }
    //}

    public partial class EditObjectView : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(EditObjectView), new PropertyMetadata());
        public EditObjectView()
        {
            InitializeComponent();

            this.WhenAnyValue(a => a.Value)
                .WhereNotNull()
                .Subscribe(a =>
                {

                    ObjectListBox.Object = JToken.FromObject(a);
                    TypesComboBox.IsEnabled = true;
                    ObjectListBox.IsEnabled = a != null;
                    SaveButton.IsEnabled = a != null;
                    if (a == null)
                    {
                        TypesComboBox.Deselect();
                    }
                });

            TypesComboBox.Changes()
                .Subscribe(change =>
                {
                    if (change is ViewModelEntity entity)
                    {
                        var type = Type.GetType(entity.Value);
                        var instance = Activator.CreateInstance(type);
                        if(instance is IInitialise initialise)
                        {
                            initialise.Initialise(default);
                        }
                        ObjectListBox.Object = JToken.FromObject(instance);
                    }
                    else
                    {

                    }
                });

            SaveButton.Clicks()
                .Subscribe(a =>
                {
                    Value = ObjectListBox.Object;
                });
        }

        private void TypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Cast<object>().Single() is Type type)
            {
                var instance = Activator.CreateInstance(type);
                ObjectListBox.Object = JToken.FromObject(instance);
            }
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }

    public class ViewModelTypesComboBox : TypesComboBox
    {
        private ViewModelTypeComboBoxService service;

        public ViewModelTypesComboBox()
        {
            //SelectedIndex = 0;
            FontWeight = FontWeights.DemiBold;
            //FontSize = 14;
            Margin = new Thickness(4);
            //Width = (this.Parent as FrameworkElement)?.ActualWidth ?? 1000;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Height = 80;
            DisplayMemberPath = nameof(ViewModelEntity.Key);
            IsCheckedPath = nameof(ViewModelEntity.IsChecked);
            IsSelectedPath = nameof(ViewModelEntity.IsSelected);
            SelectedValuePath = nameof(ViewModelEntity.Value);

            service = new ViewModelTypeComboBoxService();
            var dis = ComboBoxViewModelMapper.Connect(this, service);
            this.Unloaded += (s, e) => dis.Dispose();
        }

        public void Deselect()
        {
            service.Deselect();
        }
    }


    public class ViewModelTypeComboBoxService : ComboBoxServcice
    {
        public ViewModelTypeComboBoxService()
        {
            FreeSqlFactory.InitialiseSQLite();

            demoTypeViewModel = OutputNode<FilteredCustomCheckBoxesViewModel>.Create(() =>
            {
                var types = Helper.TypesOf<PersistViewModel>(Assembly.GetEntryAssembly());
                var filters = new Filter[] { /*new ViewModelTypeFilter()*/ };
                return UpdateAndCreate(types, filters, deselectSubject);
            });
        }
    }

    //public class ViewModelTypeFilter : PropertyFilter<AssemblyKeyValue>
    //{
    //    public ViewModelTypeFilter() : base(nameof(AssemblyKeyValue.CategoryKey))
    //    {
    //    }

    //    protected override object Set(string value)
    //    {
    //        return Enum.Parse(typeof(AssemblyType), value);
    //    }
    //}
}
