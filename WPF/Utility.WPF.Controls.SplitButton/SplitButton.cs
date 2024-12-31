
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.SplitButtons
{
    public class SplitButton : ComboBox
    {
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }


        /// <summary>
        ///     The DependencyProperty for the Content property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        //[CommonDependencyProperty]
        public static readonly DependencyProperty ContentProperty =
                DependencyProperty.Register(
                        "Content",
                        typeof(object),
                        typeof(SplitButton),
                        new FrameworkPropertyMetadata(
                                (object)null,
                                new PropertyChangedCallback(OnContentChanged)));

        /// <summary>
        ///     Content is the data used to generate the child elements of this control.
        /// </summary>
        //[Bindable(true), CustomCategory("Content")]
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        ///     Called when ContentProperty is invalidated on "d."
        /// </summary>
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton ctrl = (SplitButton)d;
            ctrl.SetValue(HasContentPropertyKey, (e.NewValue != null) ? BooleanBoxes.TrueBox : BooleanBoxes.FalseBox);

            //ctrl.OnContentChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        ///     This method is invoked when the Content property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the Content property.</param>
        /// <param name="newContent">The new value of the Content property.</param>
        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
            // Remove the old content child
            RemoveLogicalChild(oldContent);

            // if Content should not be treated as a logical child, there's
            // nothing to do
            //if (ContentIsNotLogical)
            //    return;

            //DependencyObject d = newContent as DependencyObject;
            //if (d != null)
            //{
            //    DependencyObject logicalParent = LogicalTreeHelper.GetParent(d);
            //    if (logicalParent != null)
            //    {
            //        if (TemplatedParent != null && FrameworkObject.IsEffectiveAncestor(logicalParent, this))
            //        {
            //            // In the case that this SplitButton belongs in a parent template 
            //            // and represents the content of a parent, we do not wish to change 
            //            // the logical ancestry of the content.
            //            return;
            //        }
            //        else
            //        {
            //            // If the new content was previously hooked up to the logical
            //            // tree then we sever it from the old parent. 
            //            LogicalTreeHelper.RemoveLogicalChild(logicalParent, newContent);
            //        }
            //    }
            //}

            // Add the new content child
            //AddLogicalChild(newContent);
        }

        /// <summary>
        ///     The key needed set a read-only property.
        /// </summary>
        private static readonly DependencyPropertyKey HasContentPropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "HasContent",
                        typeof(bool),
                        typeof(SplitButton),
                        new FrameworkPropertyMetadata(
                                BooleanBoxes.FalseBox,
                                FrameworkPropertyMetadataOptions.None));

        /// <summary>
        ///     The DependencyProperty for the HasContent property.
        ///     Flags:              None
        ///     Other:              Read-Only
        ///     Default Value:      false
        /// </summary>
        //[CommonDependencyProperty]
        public static readonly DependencyProperty HasContentProperty =
                HasContentPropertyKey.DependencyProperty;

        /// <summary>
        ///     True if Content is non-null, false otherwise.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public bool HasContent
        {
            get { return (bool)GetValue(HasContentProperty); }
        }

        /// <summary>
        ///     The DependencyProperty for the ContentTemplate property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        //[CommonDependencyProperty]
        public static readonly DependencyProperty ContentTemplateProperty =
                DependencyProperty.Register(
                        "ContentTemplate",
                        typeof(DataTemplate),
                        typeof(SplitButton),
                        new FrameworkPropertyMetadata(
                                (DataTemplate)null,
                              new PropertyChangedCallback(OnContentTemplateChanged)));


        /// <summary>
        ///     ContentTemplate is the template used to display the content of the control.
        /// </summary>
        //[Bindable(true), CustomCategory("Content")]
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        ///     Called when ContentTemplateProperty is invalidated on "d."
        /// </summary>
        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton ctrl = (SplitButton)d;
            //ctrl.OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        /// <summary>
        ///     This method is invoked when the ContentTemplate property changes.
        /// </summary>
        /// <param name="oldContentTemplate">The old value of the ContentTemplate property.</param>
        /// <param name="newContentTemplate">The new value of the ContentTemplate property.</param>
        protected virtual void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
        {
            //Helper.CheckTemplateAndTemplateSelector("Content", ContentTemplateProperty, ContentTemplateSelectorProperty, this);
        }

        /// <summary>
        ///     The DependencyProperty for the ContentTemplateSelector property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        //[CommonDependencyProperty]
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
                DependencyProperty.Register(
                        "ContentTemplateSelector",
                        typeof(DataTemplateSelector),
                        typeof(SplitButton),
                        new FrameworkPropertyMetadata(
                                (DataTemplateSelector)null,
                                new PropertyChangedCallback(OnContentTemplateSelectorChanged)));

        /// <summary>
        ///     ContentTemplateSelector allows the application writer to provide custom logic
        ///     for choosing the template used to display the content of the control.
        /// </summary>
        /// <remarks>
        ///     This property is ignored if <seealso cref="ContentTemplate"/> is set.
        /// </remarks>
        //[Bindable(true), CustomCategory("Content")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        /// <summary>
        ///     Called when ContentTemplateSelectorProperty is invalidated on "d."
        /// </summary>
        private static void OnContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton ctrl = (SplitButton)d;
            //ctrl.OnContentTemplateSelectorChanged((DataTemplateSelector)e.NewValue, (DataTemplateSelector)e.NewValue);
        }

        /// <summary>
        ///     This method is invoked when the ContentTemplateSelector property changes.
        /// </summary>
        /// <param name="oldContentTemplateSelector">The old value of the ContentTemplateSelector property.</param>
        /// <param name="newContentTemplateSelector">The new value of the ContentTemplateSelector property.</param>
        protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
        {
            //Helper.CheckTemplateAndTemplateSelector("Content", ContentTemplateProperty, ContentTemplateSelectorProperty, this);
        }

        /// <summary>
        ///     The DependencyProperty for the ContentStringFormat property.
        ///     Flags:              None
        ///     Default Value:      null
        /// </summary>
        //[CommonDependencyProperty]
        public static readonly DependencyProperty ContentStringFormatProperty =
                DependencyProperty.Register(
                        "ContentStringFormat",
                        typeof(String),
                        typeof(SplitButton),
                        new FrameworkPropertyMetadata(
                                (String)null,
                              new PropertyChangedCallback(OnContentStringFormatChanged)));


        /// <summary>
        ///     ContentStringFormat is the format used to display the content of
        ///     the control as a string.  This arises only when no template is
        ///     available.
        /// </summary>
        //[Bindable(true), CustomCategory("Content")]
        public String ContentStringFormat
        {
            get { return (String)GetValue(ContentStringFormatProperty); }
            set { SetValue(ContentStringFormatProperty, value); }
        }

        /// <summary>
        ///     Called when ContentStringFormatProperty is invalidated on "d."
        /// </summary>
        private static void OnContentStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton ctrl = (SplitButton)d;
            //ctrl.OnContentStringFormatChanged((String)e.OldValue, (String)e.NewValue);
        }

        /// <summary>
        ///     This method is invoked when the ContentStringFormat property changes.
        /// </summary>
        /// <param name="oldContentStringFormat">The old value of the ContentStringFormat property.</param>
        /// <param name="newContentStringFormat">The new value of the ContentStringFormat property.</param>
        protected virtual void OnContentStringFormatChanged(String oldContentStringFormat, String newContentStringFormat)
        {
        }
    }

    internal static class BooleanBoxes
    {
        internal static object TrueBox = true;
        internal static object FalseBox = false;

        internal static object Box(bool value)
        {
            if (value)
            {
                return TrueBox;
            }
            else
            {
                return FalseBox;
            }
        }
    }
}