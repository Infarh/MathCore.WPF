using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
    public static class EnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items) => new ObservableCollection<T>(items);

        public static ObservableCollection<T> ToObservableCollection<T>(this List<T> items) => new ObservableCollection<T>(items);
    }
}
