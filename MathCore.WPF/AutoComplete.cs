using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
// ReSharper disable UnusedType.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable InconsistentNaming

namespace MathCore.WPF;

/// <summary>Класс присоединённых свойств для реализации функциональности автозавершения ввода в текстовое поле TextBox</summary>
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

    /// <summary>Базовый обёртка-контрол для реализации автозавершения</summary>
    private abstract class ControlUnderAutoComplete(Control control)
    {
        internal static ControlUnderAutoComplete? Create(Control control) => control switch
        {
            ComboBox => new ComboBoxUnderAutoComplete(control),
            TextBox  => new TextBoxUnderAutoComplete(control),
            _        => null
        };

        /// <summary>Свойство зависимости, содержащее текст контролла</summary>
        public abstract DependencyProperty TextDependencyProperty { get; }

        /// <summary>Текст контролла</summary>
        public string Text
        {
            get => (string)Control.GetValue(TextDependencyProperty);
            set => Control.SetValue(TextDependencyProperty, value);
        }

        /// <summary>Вложенный WPF-контрол</summary>
        public Control Control { get; } = control;

        /// <summary>Ключ стиля для применяемого шаблона</summary>
        public abstract string StyleKey { get; }

        /// <summary>Выделить весь текст в контроле</summary>
        public abstract void SelectAll();

        /// <summary>Получить источник представления элементов из переданного стиля</summary>
        public abstract CollectionViewSource GetViewSource(Style style);
    }

    /// <summary>Реализация обёртки для TextBox</summary>
    private class TextBoxUnderAutoComplete(Control control) : ControlUnderAutoComplete(control)
    {
        /// <summary>Свойство зависимости Text для TextBox</summary>
        public override DependencyProperty TextDependencyProperty => TextBox.TextProperty;

        /// <summary>Ключ стиля для TextBox шаблона</summary>
        public override string StyleKey => "autoCompleteTextBoxStyle";

        /// <summary>Выделить весь текст в TextBox</summary>
        public override void SelectAll() => ((TextBox)Control).SelectAll();

        /// <summary>Получить CollectionViewSource из базового стиля</summary>
        public override CollectionViewSource GetViewSource(Style style) => (CollectionViewSource)style.BasedOn.Resources["viewSource"]!;
    }

    /// <summary>Реализация обёртки для ComboBox</summary>
    private class ComboBoxUnderAutoComplete(Control control) : ControlUnderAutoComplete(control)
    {
        /// <summary>Свойство зависимости Text для ComboBox</summary>
        public override DependencyProperty TextDependencyProperty => ComboBox.TextProperty;

        /// <summary>Ключ стиля для ComboBox шаблона</summary>
        public override string StyleKey => "autoCompleteComboBoxStyle";

        /// <summary>Выделить весь текст в редактируемой части ComboBox</summary>
        public override void SelectAll() => ((TextBox)Control.Template.FindName("PART_EditableTextBox", Control)).SelectAll();

        /// <summary>Получить CollectionViewSource из стиля ComboBox</summary>
        public override CollectionViewSource GetViewSource(Style style) => (CollectionViewSource)style.Resources["viewSource"]!;
    }

    /// <summary>Коллекция путей свойств для фильтра автозавершения</summary>
    [TypeConverter(typeof(AutoCompleteFilterPathCollectionTypeConverter))]
    public class AutoCompleteFilterPathCollection : Collection<string>
    {
        /// <summary>Создать коллекцию из существующего списка</summary>
        public AutoCompleteFilterPathCollection(IList<string> list) : base(list) { }

        /// <summary>Создать пустую коллекцию</summary>
        public AutoCompleteFilterPathCollection() { }

        internal string Join() => string.Join(",", this);
    }

    /// <summary>Конвертер типов для AutoCompleteFilterPathCollection из строки</summary>
    private class AutoCompleteFilterPathCollectionTypeConverter : TypeConverter
    {
        /// <summary>Проверяет возможность конвертации из указанного типа</summary>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type SourceType) => SourceType == typeof(string) || base.CanConvertFrom(context, SourceType);

        /// <summary>Проверяет возможность конвертации в указанный тип</summary>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? DestinationType) => DestinationType == typeof(string) || base.CanConvertTo(context, DestinationType);

        /// <summary>Конвертирует из строки в коллекцию путей</summary>
        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
            value is not string s
                ? base.ConvertFrom(context, culture, value)!
                : new AutoCompleteFilterPathCollection(s.Split((char[])[','], StringSplitOptions.RemoveEmptyEntries));

        /// <summary>Конвертирует коллекцию путей в строку</summary>
        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type DestinationType)
        {
            if (DestinationType != typeof(string))
                return base.ConvertTo(context, culture, value, DestinationType)!;
            var c = (AutoCompleteFilterPathCollection)value;
            return c.Join();
        }
    }

    #endregion

    #region Dependency Properties

    /// <summary>Ключ регистрационного свойства экземпляра AutoComplete</summary>
    private static readonly DependencyPropertyKey AutoCompleteInstancePropertyKey =
        DependencyProperty.RegisterAttachedReadOnly(
            "AutoCompleteInstance",
            typeof(AutoComplete),
            typeof(AutoComplete),
            new FrameworkPropertyMetadata(null));

    /// <summary>Свойство экземпляра AutoComplete</summary>
    private static readonly DependencyProperty AutoCompleteInstance = AutoCompleteInstancePropertyKey.DependencyProperty;

    private static AutoComplete? GetAutoCompleteInstance(DependencyObject o) => (AutoComplete?)o.GetValue(AutoCompleteInstance);

    /// <summary>Источник данных для автозавершения</summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.RegisterAttached(
            "Source",
            typeof(object),
            typeof(AutoComplete),
            new FrameworkPropertyMetadata(null, OnSourcePropertyChanged));

    private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (EnsureInstance(d).ViewSource is not { } view) return;
        view.Source = e.NewValue;
    }

    /// <summary>Получить значение Source</summary>
    /// <param name="d">Объект-зависимость</param>
    /// <returns>Значение источника данных</returns>
    public static object GetSource(DependencyObject d) => d.GetValue(SourceProperty);

    /// <summary>Установить значение Source</summary>
    /// <param name="d">Объект-зависимость</param>
    /// <param name="value">Новое значение</param>
    public static void SetSource(DependencyObject d, object value) => d.SetValue(SourceProperty, value);

    /// <summary>Пути фильтрации для автозавершения</summary>
    public static readonly DependencyProperty FilterPathProperty =
        DependencyProperty.RegisterAttached(
            "FilterPath",
            typeof(AutoCompleteFilterPathCollection),
            typeof(AutoComplete),
            new FrameworkPropertyMetadata(null));

    /// <summary>Получить коллекцию путей фильтра</summary>
    /// <param name="d">Объект-зависимость</param>
    /// <returns>Коллекция путей или null</returns>
    public static AutoCompleteFilterPathCollection? GetFilterPath(DependencyObject d) => (AutoCompleteFilterPathCollection?)d.GetValue(FilterPathProperty);

    /// <summary>Установить коллекцию путей фильтра</summary>
    /// <param name="d">Объект-зависимость</param>
    /// <param name="value">Коллекция путей</param>
    public static void SetFilterPath(DependencyObject d, AutoCompleteFilterPathCollection? value) => d.SetValue(FilterPathProperty, value);

    private static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.RegisterAttached(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(AutoComplete),
            new FrameworkPropertyMetadata(null, OnItemTemplatePropertyChanged));

    private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (EnsureInstance(d).ListBox is not { } list) return;
        list.ItemTemplate = (DataTemplate)e.NewValue;
    }

    /// <summary>Получить шаблон элемента списка автозавершения</summary>
    /// <param name="d">Объект-зависимость</param>
    /// <returns>Шаблон элемента</returns>
    public static DataTemplate? GetItemTemplate(DependencyObject d) => (DataTemplate?)d.GetValue(ItemTemplateProperty);

    /// <summary>Установить шаблон элемента списка автозавершения</summary>
    /// <param name="d">Объект-зависимость</param>
    /// <param name="value">Шаблон элемента</param>
    public static void SetItemTemplate(DependencyObject d, object? value) => d.SetValue(ItemTemplateProperty, value);

    private static AutoComplete EnsureInstance(DependencyObject d)
    {
        var auto_complete = GetAutoCompleteInstance(d);
        if (auto_complete != null) return auto_complete;
        auto_complete = new() { Control = (Control)d };
        d.SetValue(AutoCompleteInstancePropertyKey, auto_complete);
        return auto_complete;
    }

    #endregion

    private ControlUnderAutoComplete? _Control;

    private bool _IteratingListItems;
    private string? _RememberedText;
    private Popup? _AutoCompletePopup;

    /// <summary>Источник представления элементов коллекции</summary>
    private CollectionViewSource? ViewSource { get; set; }

    /// <summary>Ссылка на ListBox в шаблоне</summary>
    private ListBox? ListBox { get; set; }

    /// <summary>Устанавливает связанный Control и выполняет инициализацию событий и шаблона</summary>
    private Control Control
    {
        set
        {
            _Control          =  ControlUnderAutoComplete.Create(value);
            ViewSource        =  _Control!.GetViewSource((Style)this[_Control.StyleKey]);
            ViewSource.Filter += CollectionViewSource_Filter;
            value.SetValue(FrameworkElement.StyleProperty, this[_Control.StyleKey]);
            value.ApplyTemplate();
            _AutoCompletePopup = (Popup)value.Template.FindName("autoCompletePopup", value);
            ListBox            = (ListBox)value.Template.FindName("autoCompleteListBox", value);
            value.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(TextBox_TextChanged));
            value.LostFocus    += TextBox_LostFocus;
            value.PreviewKeyUp += TextBox_PreviewKeyUp;
        }
    }

    /// <summary>Создаёт компонент AutoComplete и инициализирует XAML-компоненты</summary>
    public AutoComplete() => InitializeComponent();

    /// <summary>Фильтр для элементов источника коллекции</summary>
    private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
    {
        var filter_paths = GetAutoCompleteFilterProperty();
        if (filter_paths is null) e.Accepted = TextBoxStartsWith(e.Item);
        else
        {
            var type = e.Item.GetType();
            e.Accepted = filter_paths
               .Select(AutoCompleteProperty => type.GetProperty(AutoCompleteProperty))
               .Select(info => info!.GetValue(e.Item, null))
               .Any(TextBoxStartsWith);
        }
    }

    /// <summary>Проверяет, начинается ли строковое представление значения с текущего текста</summary>
    /// <param name="value">Значение для проверки</param>
    /// <returns>True если начинается, иначе false</returns>
    private bool TextBoxStartsWith(object? value) =>
        value?.ToString()?.StartsWith(_Control!.Text, StringComparison.CurrentCultureIgnoreCase) ?? false;

    /// <summary>Получить коллекцию путей фильтра для текущего контрола</summary>
    /// <returns>Коллекция путей или null</returns>
    private AutoCompleteFilterPathCollection? GetAutoCompleteFilterProperty() => GetFilterPath(_Control!.Control);

    /// <summary>Обработчик изменения текста в контроле, обновляет видимость Popup</summary>
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_Control!.Text is not { Length: > 0 })
        {
            _AutoCompletePopup!.IsOpen = false;
            return;
        }

        if (_IteratingListItems) return;

        var v = ViewSource!.View;
        v.Refresh();
        _AutoCompletePopup!.IsOpen = !v.IsEmpty;
    }

    /// <summary>Обработчик нажатия клавиш для навигации по списку автозавершения и подтверждения выбора</summary>
    private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Up or Key.Down)
        {
            _RememberedText     ??= _Control!.Text;
            _IteratingListItems =   true;
            var view = ViewSource!.View;

            if (e.Key == Key.Up)
                if (view.CurrentItem is null)
                    view.MoveCurrentToLast();
                else
                    view.MoveCurrentToPrevious();
            else if (view.CurrentItem is null)
                view.MoveCurrentToFirst();
            else
                view.MoveCurrentToNext();
            _Control!.Text = view.CurrentItem?.ToString() ?? _RememberedText;
        }
        else
        {
            _IteratingListItems = false;
            _RememberedText     = null;
            if (!_AutoCompletePopup!.IsOpen || (e.Key != Key.Escape && e.Key != Key.Enter)) return;
            _AutoCompletePopup.IsOpen = false;
            if (e.Key == Key.Enter) _Control!.SelectAll();
        }
    }

    /// <summary>Обработчик потери фокуса, закрывает Popup</summary>
    private void TextBox_LostFocus(object sender, RoutedEventArgs e) => _AutoCompletePopup!.IsOpen = false;
}