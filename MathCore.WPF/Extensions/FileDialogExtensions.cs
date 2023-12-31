using Microsoft.Win32;

namespace MathCore.WPF.Extensions;

public static class FileDialogExtensions
{
    public static IEnumerable<(string Title, string Value)> EnumFilterValues(this FileDialog dialog)
    {
        var filter_items = dialog.Filter.Split('|').GetEnumerator();

        var i = 0;
        while (true)
        {
            i++;
            if(!filter_items.MoveNext())
                yield break;

            var title = (string)filter_items.Current;

            if (!filter_items.MoveNext())
                throw new InvalidOperationException($"Для записи фильтра {i} {title} отсутствует значение");

            var value = (string)filter_items.Current;

            yield return (title, value);
        }
    }
}
