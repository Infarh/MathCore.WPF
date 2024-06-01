using System.Windows;
using System.Windows.Data;

namespace MathCore.WPF;

public class ValueFilterItem : CollectionViewFilterItem
{
    #region FilterText : string - Текст фильтра

    /// <summary>Текст фильтра</summary>
    public static readonly DependencyProperty FilterTextProperty =
        DependencyProperty.Register(
            nameof(FilterText),
            typeof(string),
            typeof(ValueFilterItem),
            new(default(string)));

    /// <summary>Текст фильтра</summary>
    public string? FilterText
    {
        get => (string)GetValue(FilterTextProperty);
        set => SetValue(FilterTextProperty, value);
    }

    #endregion

    protected override Freezable CreateInstanceCore() => new ValueFilterItem { FilterText = FilterText };

    protected override void OnFilter(object Sender, FilterEventArgs E)
    {
        var filter_text = FilterText;
        if (string.IsNullOrEmpty(filter_text)) return;
    }
}