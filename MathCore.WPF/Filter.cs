using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MathCore.WPF;

// Defines SearchPredicate delegate alias as <element, search pattern, result>
/// <summary>
/// Provides a search/filter for items bind to an ItemsControl.
/// To use this control, simply place an ItemsControl object as the content
/// </summary>
[TemplatePart(Name = "PART_FilterBox")]
public class Filter : HeaderedContentControl
{
    static Filter() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Filter), new FrameworkPropertyMetadata(typeof(Filter)));

    public static readonly Func<object, string, bool> ContentTextSearch = (element, pattern) =>
    {
        if(string.IsNullOrEmpty(pattern)) return true;
        var container = (ContentControl)element;
        return (container.Content?.ToString() ?? element.ToString()).ToLower().Contains(pattern.ToLower());
    };

    public static readonly DependencyProperty FilterBoxStyleProperty = DependencyProperty.Register(nameof(FilterBoxStyle), typeof(Style), typeof(Filter), new FrameworkPropertyMetadata(null, (_, _) => { }));
    public Style FilterBoxStyle { get => (Style)GetValue(FilterBoxStyleProperty); set => SetValue(FilterBoxStyleProperty, value); }

    public static readonly DependencyProperty PatternProperty = DependencyProperty.Register(nameof(Pattern), typeof(string), typeof(Filter), new FrameworkPropertyMetadata(string.Empty, (s, _) => ((Filter)s).View.Refresh()));
    public string Pattern { get => (string)GetValue(PatternProperty); set => SetValue(PatternProperty, value); }

    public static readonly DependencyProperty SearchStrategyProperty = DependencyProperty.Register(nameof(SearchStrategy), typeof(Func<object, string, bool>), typeof(Filter), new FrameworkPropertyMetadata(ContentTextSearch, OnSearchStrategyChanged));

    public Func<object, string, bool> SearchStrategy { get => (Func<object, string, bool>)GetValue(SearchStrategyProperty); set => SetValue(SearchStrategyProperty, value); }

    private static void OnSearchStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var f = (Filter)d;
        if(f.View is null) return;
        f.View.Filter = i => f.SearchStrategy(i, f.Pattern);
    }

    private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(Filter), new FrameworkPropertyMetadata(null, OnItemsSourceChanged));

    private IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if(e.OldValue is IEnumerable old)
            CollectionViewSource.GetDefaultView(@old).Filter = null;

        if(e.NewValue is not IEnumerable @new) return;
        var f = (Filter)d;
        CollectionViewSource.GetDefaultView(@new).Filter = i => f.SearchStrategy(i, f.Pattern);
    }

    private TextBox _FilterBox;

    public Filter() => FilterBoxStyle = (Style)TryFindResource(new ComponentResourceKey(typeof(Filter), "FilterBoxStyle"));

    private ICollectionView? View => CollectionViewSource.GetDefaultView(ItemsSource);

    protected override void OnContentChanged(object OldContent, object NewContent)
    {
        if(NewContent is not ItemsControl control)
            throw new ArgumentException("Content or Content Template must be an ItemsControl");

        SetBinding(ItemsSourceProperty, new Binding("ItemsSource") { Mode = BindingMode.OneWay, Source = control });

        base.OnContentChanged(OldContent, NewContent);
    }

    public override void OnApplyTemplate()
    {
        _FilterBox = Template.FindName("PART_FilterBox", this) as TextBox;

        if(_FilterBox is null)
            throw new ArgumentException("Filter ControlTemplate must have at least one TextBox element named PART_FilterBox");

        SetBinding(PatternProperty, new Binding("Text") { Mode = BindingMode.TwoWay, Source = _FilterBox });

        base.OnApplyTemplate();
    }
}