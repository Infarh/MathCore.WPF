using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Collections.ObjectModel
{
    public static class EnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items) => new(items);

        public static ObservableCollection<T> ToObservableCollection<T>(this List<T> items) => new(items);
    }
}
