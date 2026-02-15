using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MathCore.WPF;

/// <summary>Предоставляет возможность поиска/фильтрации элементов, привязанных к ItemsControl. Для использования этого элемента управления просто поместите объект ItemsControl в качестве содержимого</summary>
[TemplatePart(Name = "PART_FilterBox")]
public class Filter : HeaderedContentControl
{
    static Filter() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Filter), new FrameworkPropertyMetadata(typeof(Filter)));

    /// <summary>Стратегия поиска по умолчанию, выполняющая текстовый поиск по содержимому элемента</summary>
    public static readonly Func<object, string, bool> ContentTextSearch = (element, pattern) =>
    {
        if(string.IsNullOrEmpty(pattern)) return true;
        var container = (ContentControl)element;
        return (container.Content?.ToString() ?? element.ToString()).ToLower().Contains(pattern.ToLower());
    };

    /// <summary>Свойство зависимости для стиля текстового поля фильтра</summary>
    public static readonly DependencyProperty FilterBoxStyleProperty = DependencyProperty.Register(nameof(FilterBoxStyle), typeof(Style), typeof(Filter), new FrameworkPropertyMetadata(null, (_, _) => { }));
    
    /// <summary>Стиль текстового поля фильтра</summary>
    public Style FilterBoxStyle { get => (Style)GetValue(FilterBoxStyleProperty); set => SetValue(FilterBoxStyleProperty, value); }

    /// <summary>Свойство зависимости для шаблона поиска</summary>
    public static readonly DependencyProperty PatternProperty = DependencyProperty.Register(nameof(Pattern), typeof(string), typeof(Filter), new FrameworkPropertyMetadata(string.Empty, (s, _) => ((Filter)s).View.Refresh()));
    
    /// <summary>Шаблон поиска для фильтрации элементов</summary>
    public string Pattern { get => (string)GetValue(PatternProperty); set => SetValue(PatternProperty, value); }

    /// <summary>Свойство зависимости для стратегии поиска</summary>
    public static readonly DependencyProperty SearchStrategyProperty = DependencyProperty.Register(nameof(SearchStrategy), typeof(Func<object, string, bool>), typeof(Filter), new FrameworkPropertyMetadata(ContentTextSearch, OnSearchStrategyChanged));

    /// <summary>Стратегия поиска, определяющая, соответствует ли элемент заданному шаблону</summary>
    public Func<object, string, bool> SearchStrategy { get => (Func<object, string, bool>)GetValue(SearchStrategyProperty); set => SetValue(SearchStrategyProperty, value); }

    /// <summary>Обработчик изменения стратегии поиска</summary>
    /// <param name="d">Объект зависимости</param>
    /// <param name="e">Аргументы события изменения свойства</param>
    private static void OnSearchStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var f = (Filter)d;
        if(f.View is null) return;
        f.View.Filter = i => f.SearchStrategy(i, f.Pattern);
    }

    /// <summary>Свойство зависимости для источника элементов</summary>
    private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(Filter), new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

    /// <summary>Источник элементов для фильтрации</summary>
    private IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    /// <summary>Обработчик изменения источника элементов</summary>
    /// <param name="d">Объект зависимости</param>
    /// <param name="e">Аргументы события изменения свойства</param>
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if(e.OldValue is IEnumerable old)
            CollectionViewSource.GetDefaultView(@old).Filter = null;

        if(e.NewValue is not IEnumerable @new) return;
        var f = (Filter)d;
        CollectionViewSource.GetDefaultView(@new).Filter = i => f.SearchStrategy(i, f.Pattern);
    }

    private TextBox _FilterBox; // Текстовое поле фильтра

    /// <summary>Инициализирует новый экземпляр класса Filter</summary>
    public Filter() => FilterBoxStyle = (Style)TryFindResource(new ComponentResourceKey(typeof(Filter), "FilterBoxStyle"));

    /// <summary>Представление коллекции для фильтрации</summary>
    private ICollectionView? View => CollectionViewSource.GetDefaultView(ItemsSource);

    /// <summary>Вызывается при изменении содержимого элемента управления</summary>
    /// <param name="OldContent">Старое содержимое</param>
    /// <param name="NewContent">Новое содержимое</param>
    /// <exception cref="ArgumentException">Содержимое или шаблон содержимого должны быть типа ItemsControl</exception>
    protected override void OnContentChanged(object OldContent, object NewContent)
    {
        if(NewContent is not ItemsControl control)
            throw new ArgumentException("Content or Content Template must be an ItemsControl");

        SetBinding(ItemsSourceProperty, new Binding("ItemsSource") { Mode = BindingMode.OneWay, Source = control });

        base.OnContentChanged(OldContent, NewContent);
    }

    /// <summary>Вызывается при применении шаблона элемента управления</summary>
    /// <exception cref="ArgumentException">Шаблон элемента управления Filter должен содержать хотя бы один элемент TextBox с именем PART_FilterBox</exception>
    public override void OnApplyTemplate()
    {
        _FilterBox = Template.FindName("PART_FilterBox", this) as TextBox;

        if(_FilterBox is null)
            throw new ArgumentException("Filter ControlTemplate must have at least one TextBox element named PART_FilterBox");

        SetBinding(PatternProperty, new Binding("Text") { Mode = BindingMode.TwoWay, Source = _FilterBox });

        base.OnApplyTemplate();
    }
}