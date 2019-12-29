using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace MathCore.WPF
{
    /// <summary>Сласс присоединённых своёст для реализации функциональности автозавершения ввода в текстовое поле TextBox</summary>
    /// <example>
    /// <StackPanel VerticalAlignment="Center">
    ///    <StackPanel.Resources>
    ///       <x:Array x:Key="_Source" Type="s:String">
    ///          <s:String>Str1</s:String>
    ///          <s:String>Str2</s:String>
    ///          <s:String>Str3</s:String>
    ///          <s:String>qwe1</s:String>
    ///          <s:String>qw3e</s:String>
    ///       </x:Array>
    ///    </StackPanel.Resources>
    ///    <TextBox AutoComplete.Source="{StaticResource _Source}"/>
    /// </StackPanel>
    /// </example>
    public sealed partial class AutoComplete
    {
        #region Classes

        private abstract class ControlUnderAutoComplete
        {
            internal static ControlUnderAutoComplete Create(Control control) => control is ComboBox
                ? (ControlUnderAutoComplete) new ComboBoxUnderAutoComplete(control)
                : (control is TextBox ? new TextBoxUnderAutoComplete(control) : null);

            public abstract DependencyProperty TextDependencyProperty { get; }

            public string Text
            {
                get => (string)Control.GetValue(TextDependencyProperty);
                set => Control.SetValue(TextDependencyProperty, value);
            }

            public Control Control { get; private set; }
            public abstract string StyleKey { get; }
            protected ControlUnderAutoComplete(Control control) { Control = control; }

            public abstract void SelectAll();

            public abstract CollectionViewSource GetViewSource(Style style);
        }

        private class TextBoxUnderAutoComplete : ControlUnderAutoComplete
        {

            public TextBoxUnderAutoComplete(Control control) : base(control) { }

            public override DependencyProperty TextDependencyProperty => TextBox.TextProperty;

            public override string StyleKey => "autoCompleteTextBoxStyle";


            public override void SelectAll() => ((TextBox)Control).SelectAll();

            public override CollectionViewSource GetViewSource(Style style) => (CollectionViewSource)style.BasedOn.Resources["viewSource"];
        }

        private class ComboBoxUnderAutoComplete : ControlUnderAutoComplete
        {

            public ComboBoxUnderAutoComplete(Control control) : base(control) { }

            public override DependencyProperty TextDependencyProperty => ComboBox.TextProperty;

            public override string StyleKey => "autoCompleteComboBoxStyle";


            public override void SelectAll() => ((TextBox)Control.Template.FindName("PART_EditableTextBox", Control)).SelectAll();

            public override CollectionViewSource GetViewSource(Style style) => (CollectionViewSource)style.Resources["viewSource"];
        }

        [TypeConverter(typeof(AutoCompleteFilterPathCollectionTypeConverter))]
        public class AutoCompleteFilterPathCollection : Collection<string>
        {
            public AutoCompleteFilterPathCollection(IList<string> list) : base(list) { }

            public AutoCompleteFilterPathCollection() { }

            internal string Join()
            {
                var array = new string[Count];
                CopyTo(array, 0);
                return string.Join(",", array);
            }
        }

        private class AutoCompleteFilterPathCollectionTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                var s = value as string;
                return s == null ? base.ConvertFrom(context, culture, value) : new AutoCompleteFilterPathCollection(s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType != typeof(string)) return base.ConvertTo(context, culture, value, destinationType);
                var c = (AutoCompleteFilterPathCollection)value;
                return c.Join();
            }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyPropertyKey AutoCompleteInstancePropertyKey = 
            DependencyProperty.RegisterAttachedReadOnly(
                "AutoCompleteInstance",
                typeof(AutoComplete),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));


        private static readonly DependencyProperty AutoCompleteInstance = AutoCompleteInstancePropertyKey.DependencyProperty;

        private static AutoComplete GetAutoCompleteInstance(DependencyObject o) => (AutoComplete)o.GetValue(AutoCompleteInstance);

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                "Source",
                typeof(object),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnSourcePropertyChanged));

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => EnsureInstance(d).ViewSource.Source = e.NewValue;

        public static object GetSource(DependencyObject d) => d.GetValue(SourceProperty);

        public static void SetSource(DependencyObject d, object value) => d.SetValue(SourceProperty, value);

        public static readonly DependencyProperty FilterPathProperty =
            DependencyProperty.RegisterAttached(
                "FilterPath",
                typeof(AutoCompleteFilterPathCollection),
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null));

        public static AutoCompleteFilterPathCollection GetFilterPath(DependencyObject d) => (AutoCompleteFilterPathCollection)d.GetValue(FilterPathProperty);

        public static void SetFilterPath(DependencyObject d, AutoCompleteFilterPathCollection value) => d.SetValue(FilterPathProperty, value);

        private static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.RegisterAttached(
                "ItemTemplate",
                typeof(DataTemplate), 
                typeof(AutoComplete),
                new FrameworkPropertyMetadata(null, OnItemTemplatePropertyChanged));

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => EnsureInstance(d).ListBox.ItemTemplate = (DataTemplate)e.NewValue;

        public static DataTemplate GetItemTemplate(DependencyObject d) => (DataTemplate)d.GetValue(ItemTemplateProperty);

        public static void SetItemTemplate(DependencyObject d, object value) => d.SetValue(ItemTemplateProperty, value);

        private static AutoComplete EnsureInstance(DependencyObject d)
        {
            var auto_complite = GetAutoCompleteInstance(d);
            if(auto_complite != null) return auto_complite;
            auto_complite = new AutoComplete { Control = (Control)d };
            d.SetValue(AutoCompleteInstancePropertyKey, auto_complite);
            return auto_complite;
        }

        #endregion

        private ControlUnderAutoComplete _Control;

        private bool _IteratingListItems;
        private string _RememberedText;
        private Popup _AutoCompletePopup;

        private CollectionViewSource ViewSource { get; set; }

        private ListBox ListBox { get; set; }

        private Control Control
        {
            set
            {
                _Control = ControlUnderAutoComplete.Create(value);
                ViewSource = _Control.GetViewSource((Style)this[_Control.StyleKey]);
                ViewSource.Filter += CollectionViewSource_Filter;
                value.SetValue(FrameworkElement.StyleProperty, this[_Control.StyleKey]);
                value.ApplyTemplate();
                _AutoCompletePopup = (Popup)value.Template.FindName("autoCompletePopup", value);
                ListBox = (ListBox)value.Template.FindName("autoCompleteListBox", value);
                value.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(TextBox_TextChanged));
                value.LostFocus += TextBox_LostFocus;
                value.PreviewKeyUp += TextBox_PreviewKeyUp;
            }
        }

        public AutoComplete() { InitializeComponent(); }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var filter_paths = GetAutoCompleteFilterProperty();
            if(filter_paths == null) e.Accepted = TextBoxStartsWith(e.Item);
            else
            {
                var type = e.Item.GetType();
                e.Accepted = filter_paths
                            .Select(autoCompleteProperty => type.GetProperty(autoCompleteProperty))
                            .Select(info => info.GetValue(e.Item, null))
                            .Any(TextBoxStartsWith);
            }
        }

        private bool TextBoxStartsWith(object value) => value != null && value.ToString().StartsWith(_Control.Text, StringComparison.CurrentCultureIgnoreCase);

        private AutoCompleteFilterPathCollection GetAutoCompleteFilterProperty() => GetFilterPath(_Control.Control);

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(_Control.Text == "")
            {
                _AutoCompletePopup.IsOpen = false;
                return;
            }
            if(_IteratingListItems || _Control.Text == "") return;
            var v = ViewSource.View;
            v.Refresh();
            _AutoCompletePopup.IsOpen = !v.IsEmpty;
        }

        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Up || e.Key == Key.Down)
            {
                if(_RememberedText == null) _RememberedText = _Control.Text;
                _IteratingListItems = true;
                var view = ViewSource.View;

                if (e.Key == Key.Up)
                    if (view.CurrentItem == null)
                        view.MoveCurrentToLast();
                    else
                        view.MoveCurrentToPrevious();
                else if (view.CurrentItem == null)
                    view.MoveCurrentToFirst();
                else
                    view.MoveCurrentToNext();
                _Control.Text = view.CurrentItem?.ToString() ?? _RememberedText;
            }
            else
            {
                _IteratingListItems = false;
                _RememberedText = null;
                if(!_AutoCompletePopup.IsOpen || (e.Key != Key.Escape && e.Key != Key.Enter)) return;
                _AutoCompletePopup.IsOpen = false;
                if(e.Key == Key.Enter) _Control.SelectAll();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => _AutoCompletePopup.IsOpen = false;
    }
}
