using System.Windows;
using System.Windows.Data;

namespace MathCore.WPF;

public class RangeCollectionFilterItem : CollectionViewFilterItem
{
    #region Min - минимум фильтра

    /// <summary>Свойство минимального фильтруемого значения</summary>
    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register(
            nameof(Min),
            typeof(IComparable),
            typeof(RangeCollectionFilterItem),
            new PropertyMetadata(default(IComparable), RefreshSource));

    /// <summary>Свойство минимального фильтруемого значения</summary>
    public IComparable? Min
    {
        get => (IComparable)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    #endregion

    #region MinInclude - включать нижний предел в выборку

    /// <summary>Включать нижний предел в выборку</summary>
    public static readonly DependencyProperty MinIncludeProperty =
        DependencyProperty.Register(
            nameof(MinInclude),
            typeof(bool),
            typeof(RangeCollectionFilterItem),
            new PropertyMetadata(true, RefreshSource));

    /// <summary>Включать нижний предел в выборку</summary>
    public bool MinInclude
    {
        get => (bool)GetValue(MinIncludeProperty); 
        set => SetValue(MinIncludeProperty, value);
    }

    #endregion

    #region Max - максимум фильтра

    /// <summary>Свойство максимума фильтра</summary>
    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register(
            nameof(Max),
            typeof(IComparable),
            typeof(RangeCollectionFilterItem),
            new PropertyMetadata(default(IComparable), RefreshSource));

    /// <summary>Свойство максимума фильтра</summary>
    public IComparable? Max
    {
        get => (IComparable)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    #endregion

    #region MaxInclude - включать верхний предел в выборку

    /// <summary>Включать верхний предел в выборку</summary>
    public static readonly DependencyProperty MaxIncludeProperty =
        DependencyProperty.Register(
            nameof(MaxInclude),
            typeof(bool),
            typeof(RangeCollectionFilterItem),
            new PropertyMetadata(true, RefreshSource));

    /// <summary>Включать верхний предел в выборку</summary>
    public bool MaxInclude
    {
        get => (bool)GetValue(MaxIncludeProperty); 
        set => SetValue(MaxIncludeProperty, value);
    }

    #endregion

    /// <inheritdoc />
    protected override Freezable CreateInstanceCore()
    {
        var filter = new RangeCollectionFilterItem();
        filter.SetSource(_Source);
        return filter;
    }

    /// <inheritdoc />
    protected override void OnFilter(object Sender, FilterEventArgs E)
    {
        if (!E.Accepted || !Enabled)
            return;

        var value = GetItemValue(E.Item);
        if (Min is { } min)
            if (MinInclude)
            {
                if (min.CompareTo(value) > 0)
                {
                    E.Accepted = false;
                    return;
                }
            }
            else if (min.CompareTo(value) >= 0)
            {
                E.Accepted = false;
                return;
            }

        //var max = ;
        if (Max is not { } max) return;
        if (MaxInclude)
        {
            if (max.CompareTo(value) < 0) E.Accepted = false;
        }
        else if (max.CompareTo(value) <= 0) E.Accepted = false;
    }
}