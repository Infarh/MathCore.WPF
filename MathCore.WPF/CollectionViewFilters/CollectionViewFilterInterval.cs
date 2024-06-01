using System.Windows.Data;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.CollectionViewFilters;

public class CollectionViewFilterInterval<TItem, TCriteria> : ViewModel where TCriteria : IComparable
{
    private readonly CollectionViewSource _View;

    private readonly Func<TItem, TCriteria> _Selector;

    #region Minimum : TCriteria - Минимальное допустимое значение

    /// <summary>Минимальное допустимое значение</summary>
    private TCriteria _Minimum;

    /// <summary>Минимальное допустимое значение</summary>
    public TCriteria Minimum
    {
        get => _Minimum;
        set => Set(ref _Minimum, value);
    }

    #endregion

    #region Maximum : TCriteria - Максимальное допустимое значение

    /// <summary>Максимальное допустимое значение</summary>
    private TCriteria _Maximum;

    /// <summary>Максимальное допустимое значение</summary>
    public TCriteria Maximum
    {
        get => _Maximum; 
        set => Set(ref _Maximum, value);
    }

    #endregion

    #region Enabled : bool - Фильтр активен

    /// <summary>Фильтр активен</summary>
    private bool _Enabled;

    /// <summary>Фильтр активен</summary>
    public bool Enabled
    {
        get => _Enabled;
        set => Set(ref _Enabled, value);
    }

    #endregion

    public string Name { get; }

    public CollectionViewFilterInterval(CollectionViewSource view, Func<TItem, TCriteria> selector, string? Name = null)
    {
        this.Name = Name;
        _View = view;
        _Selector = selector;
        view.Filter += FilterItems;
    }

    private static void FilterItems(object Sender, FilterEventArgs E)
    {
    }
}