using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using ColorCode.Compilation.Languages;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Structs;
using Utility.WPF.Factorys;
using Orientation = Utility.Enums.Orientation;

namespace Utility.WPF.Attached
{
    public static class ItemsPanelHelpers
    {
        public static readonly DependencyProperty IsLoadedSetProperty =
    DependencyProperty.RegisterAttached(
        "IsLoadedSet",
        typeof(bool),
        typeof(ItemsPanelHelpers),
        new PropertyMetadata());

        public static bool GetIsLoadedSet(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLoadedSetProperty);
        }

        public static void SetIsLoadedSet(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLoadedSetProperty, value);
        }



        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.RegisterAttached(
                "Orientation",
                typeof(Orientation),
                typeof(ItemsPanelHelpers),
                new PropertyMetadata(Orientation.Vertical, onChanged));

        public static void SetOrientation(DependencyObject element, Orientation value) =>
            element.SetValue(OrientationProperty, value);

        public static Orientation GetOrientation(DependencyObject element) =>
            (Orientation)element.GetValue(OrientationProperty);

        public static readonly DependencyProperty ArrangementProperty =
                    DependencyProperty.RegisterAttached(
                        "Arrangement",
                typeof(Arrangement), // or your enum
                typeof(ItemsPanelHelpers),
                new PropertyMetadata(default(Arrangement), onChanged));

        public static void SetArrangement(DependencyObject element, Arrangement value) =>
            element.SetValue(ArrangementProperty, value);

        public static Arrangement GetArrangement(DependencyObject element) =>
            (Arrangement)element.GetValue(ArrangementProperty);

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached(
       "Rows",
       typeof(IReadOnlyCollection<Dimension>),
       typeof(ItemsPanelHelpers),
       new PropertyMetadata(null, onChanged));

        public static void SetRows(DependencyObject element, IReadOnlyCollection<Dimension> value) =>
            element.SetValue(RowsProperty, value);

        public static IReadOnlyCollection<Dimension> GetRows(DependencyObject element) =>
            (IReadOnlyCollection<Dimension>)element.GetValue(RowsProperty);

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached(
                "Columns",
                typeof(IReadOnlyCollection<Dimension>),
                typeof(ItemsPanelHelpers),
                new PropertyMetadata(null, onChanged));

        public static void SetColumns(DependencyObject element, IReadOnlyCollection<Dimension> value) =>
            element.SetValue(ColumnsProperty, value);

        public static IReadOnlyCollection<Dimension> GetColumns(DependencyObject element) =>
            (IReadOnlyCollection<Dimension>)element.GetValue(ColumnsProperty);


        public static readonly DependencyProperty ItemsPanelTemplateProperty =
            DependencyProperty.RegisterAttached(
                "ItemsPanelTemplate",
                typeof(ItemsPanelTemplate),
                typeof(ItemsPanelHelpers),
                new PropertyMetadata(null));

        public static void SetItemsPanelTemplate(DependencyObject element, ItemsPanelTemplate value) =>
            element.SetValue(ItemsPanelTemplateProperty, value);

        public static ItemsPanelTemplate GetItemsPanelTemplate(DependencyObject element) =>
            (ItemsPanelTemplate)element.GetValue(ItemsPanelTemplateProperty);



        private static void onChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl ic)
            {
                if (ic.IsLoaded)
                {
                    load(true);
                }
                else if (GetIsLoadedSet(ic) == false)
                {
                    SetIsLoadedSet(ic, true);
                    ic.Loaded += (s, e) => load(true);
                }

                void load(bool b)
                {
                    var rows = GetRows(ic);
                    var columns = GetColumns(ic);
                    var orientation = GetOrientation(ic) switch
                    {
                        Orientation.Horizontal => System.Windows.Controls.Orientation.Horizontal,
                        Orientation.Vertical => System.Windows.Controls.Orientation.Vertical,
                        Orientation.None => System.Windows.Controls.Orientation.Vertical
                    };
                    var arrangement = GetArrangement(ic);

                    // Call your converter or logic to create the ItemsPanelTemplate
                    if (b && ic.ItemsPanel != null)
                        ic.ItemsPanel = ItemsPanelFactory.Template(
                        rows,
                        columns,
                        orientation,
                        arrangement);
                }
            }
        }
    }
}
