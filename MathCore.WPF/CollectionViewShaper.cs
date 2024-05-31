using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Data;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

/// <summary> </summary>
/// <remarks>
/// <code>
/// // Collection to which the view is bound
///    public ObservableCollection People { get; private set; }
///    ...
///
/// // Default view of the People collection
///    ICollectionView view = CollectionViewSource.GetDefaultView(People);
///
///    // Show only adults
///    view.Filter = o => ((Person) o).Age >= 18;
///
/// // Sort by last name and first name
///    view.SortDescriptions.Add(new SortDescription("LastName", ListSortDirection.Ascending));
///    view.SortDescriptions.Add(new SortDescription("FirstName", ListSortDirection.Ascending));
///
/// // Group by country
///    view.GroupDescriptions.Add(new PropertyGroupDescription("Country"));
///
/// People.Where(p => p.Age >= 18).OrderBy(p => p.LastName).ThenBy(p => p.FirstName).GroupBy(p => p.Country);
/// 
/// from p in People
///        where p.Age >= 18
///        orderby p.LastName, p.FirstName
///        group p by p.Country;
/// 
/// var query = from p in People.ShapeView()
///                    where p.Age >= 18
///                    orderby p.LastName, p.FirstName
///                    group p by p.Country;
/// query.Apply()
/// 
/// // Remove the grouping and add a sort criteria
/// People.ShapeView()
///   .ClearGrouping()
///   .OrderBy(p => p.LastName);
///   .Apply();
///  
/// </code>
/// </remarks>
[Copyright("Thomas Levesque", url = "http://www.thomaslevesque.com/2011/11/30/wpf-using-linq-to-shape-data-in-a-collectionview/")]
public static class CollectionViewShaper
{
    public static CollectionViewShaper<T> ShapeView<T>(this IEnumerable<T> source) => new(CollectionViewSource.GetDefaultView(source));

    public static CollectionViewShaper<T> Shape<T>(this ICollectionView view) => new(view);
}

public class CollectionViewShaper<T>
{
    private readonly ICollectionView _View;
    private Predicate<object>? _Filter;
    private readonly List<SortDescription> _SortDescriptions;
    private readonly List<GroupDescription> _GroupDescriptions;

    public CollectionViewShaper(ICollectionView view)
    {
        _View              = view ?? throw new ArgumentNullException(nameof(view));
        _Filter            = view.Filter;
        _SortDescriptions  = view.SortDescriptions.ToList();
        _GroupDescriptions = view.GroupDescriptions.ToList();
    }

    public void Apply()
    {
        using (_View.DeferRefresh())
        {
            _View.Filter = _Filter;
            _View.SortDescriptions.Clear();
            _View.SortDescriptions.AddItems(_SortDescriptions);
            _View.GroupDescriptions.Clear();
            _View.GroupDescriptions.AddItems(_GroupDescriptions);
        }
    }

    public CollectionViewShaper<T> ClearGrouping()
    {
        _GroupDescriptions.Clear();
        return this;
    }

    public CollectionViewShaper<T> ClearSort()
    {
        _SortDescriptions.Clear();
        return this;
    }

    public CollectionViewShaper<T> ClearFiler()
    {
        _Filter = null;
        return this;
    }

    public CollectionViewShaper<T> ClearAll()
    {
        _Filter = null;
        _GroupDescriptions.Clear();
        _SortDescriptions.Clear();
        return this;
    }

    public CollectionViewShaper<T> Where(Func<T, bool> predicate)
    {
        _Filter = o => predicate((T)o);
        return this;
    }

    public CollectionViewShaper<T> OrderBy<TKey>(Expression<Func<T, TKey>> selector) => Order(selector, ListSortDirection.Ascending, true);
    public CollectionViewShaper<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> selector) => Order(selector, ListSortDirection.Descending, true);
    public CollectionViewShaper<T> ThenBy<TKey>(Expression<Func<T, TKey>> selector) => Order(selector);
    public CollectionViewShaper<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> selector) => Order(selector, ListSortDirection.Descending);

    private CollectionViewShaper<T> Order<TKey>(Expression<Func<T, TKey>> selector, ListSortDirection direction = ListSortDirection.Ascending, bool clear = false)
    {
        var path = GetPropertyPath(selector.Body);
        if (clear) _SortDescriptions.Clear();
        _SortDescriptions.Add(new(path, direction));
        return this;
    }

    public CollectionViewShaper<T> GroupBy<TKey>(Expression<Func<T, TKey>> selector)
    {
        _GroupDescriptions.Add(new PropertyGroupDescription(GetPropertyPath(selector)));
        return this;
    }

    private static string GetPropertyPath(Expression expression)
    {
        var names = new Stack<string>();
        var expr  = expression;
        while (expr is { } and not ParameterExpression and not ConstantExpression)
        {
            if (expr is not MemberExpression member)
                throw new ArgumentException("The selector body must contain only property or field access expressions", nameof(expression));
            names.Push(member.Member.Name);
            expr = member.Expression;
        }

        return string.Join(".", names);
    }
}