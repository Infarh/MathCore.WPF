using System.ComponentModel;

namespace MathCore.WPF.Extensions;

public static class NotifyPropertyChangedEx
{
    public static T OnPropertyChanged<T>(this T obj, string PropertyName, Action<T> action)
    {
        if (obj is INotifyPropertyChanged propertyChanged)
            propertyChanged.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == PropertyName)
                    action((T)sender!);
            };
        return obj;
    }

    public static T OnPropertyChanged<T>(this T obj, string PropertyName, Func<T, bool> condition, Action<T> action)
    {
        if (obj is INotifyPropertyChanged propertyChanged)
            propertyChanged.PropertyChanged += (sender, args) =>
            {
                var value = (T)sender!;
                if (args.PropertyName == PropertyName && condition(value))
                    action(value);
            };
        return obj;
    }

    public static T OnPropertyChanged<T>(this T obj, string PropertyName, PropertyChangedEventHandler PropertyChangedHandler)
       where T : class, INotifyPropertyChanged
    {
        obj.SubscribeTo(PropertyName, PropertyChangedHandler);

        return obj;
    }
}