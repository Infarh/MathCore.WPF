using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

public static class CollectionFiltersExtensions
{
    public static CollectionFilter<TValue, TCriteria> Filter<TValue, TCriteria>(
        this ObservableCollection<TValue?> collection,
        Func<TValue, TCriteria> selector) => 
        new(collection, selector);

    public static CollectionViewFilter<TCriteria> FilterView<TCriteria>(
        this ICollectionView view,
        Func<object, TCriteria> selector,
        string? Name = null) where TCriteria : notnull =>
        new(view, selector, Name);

    public static CollectionViewFilter<TCriteria, TItem> FilterView<TItem, TCriteria>(
        this ICollectionView view,
        Func<TItem, TCriteria> selector,
        string? Name = null) where TCriteria : notnull => 
        new(view, selector, Name);

    public static CollectionViewFilter<TCriteria> FilterView<TCriteria>(
        this CollectionViewSource source,
        Func<object, TCriteria> selector,
        string? Name = null)
        where TCriteria : notnull
    {
        var filter = source.View.FilterView(selector, Name);
        source.Filter += filter.Filter;
        return filter;
    }

    public static CollectionViewFilter<TCriteria, TItem> FilterView<TItem, TCriteria>(
        this CollectionViewSource source,
        Func<TItem, TCriteria> selector,
        string? Name = null) where TCriteria : notnull
    {
        var filter = source.View.FilterView(selector, Name);
        source.Filter += filter.Filter;
        return filter;
    }
}