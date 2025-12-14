using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MathCore.WPF.Notifications;

/// <summary>Конвертер типа иконки уведомления в визуальное представление</summary>
[ValueConversion(typeof(ToastNotificationIcon), typeof(object))]
public class ToastNotificationIconConverter : IValueConverter
{
    public object? Convert(object? Value, Type TargetType, object? Parameter, CultureInfo Culture)
    {
        if (Value is not ToastNotificationIcon icon)
            return null;

        var parameter_str = Parameter as string;

        if (parameter_str == "Color")
            return GetIconColor(icon);

        if (parameter_str == "Path")
            return GetIconPath(icon);

        return null;
    }

    public object ConvertBack(object? Value, Type TargetType, object? Parameter, CultureInfo Culture) =>
        throw new NotSupportedException();

    private static Brush GetIconColor(ToastNotificationIcon Icon) => Icon switch
    {
        ToastNotificationIcon.Information => new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD7)),
        ToastNotificationIcon.Success => new SolidColorBrush(Color.FromRgb(0x10, 0x79, 0x3F)),
        ToastNotificationIcon.Warning => new SolidColorBrush(Color.FromRgb(0xFF, 0x8C, 0x00)),
        ToastNotificationIcon.Error => new SolidColorBrush(Color.FromRgb(0xE8, 0x11, 0x23)),
        ToastNotificationIcon.Critical => new SolidColorBrush(Color.FromRgb(0xC5, 0x00, 0x00)),
        ToastNotificationIcon.Question => new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD7)),
        _ => new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80))
    };

    private static string GetIconPath(ToastNotificationIcon Icon) => Icon switch
    {
        // Информация (i в круге)
        ToastNotificationIcon.Information => 
            "M 50,0 C 22.4,0 0,22.4 0,50 0,77.6 22.4,100 50,100 77.6,100 100,77.6 100,50 100,22.4 77.6,0 50,0 Z M 50,80 C 47,80 45,78 45,75 L 45,45 C 45,42 47,40 50,40 53,40 55,42 55,45 L 55,75 C 55,78 53,80 50,80 Z M 50,30 C 47,30 45,28 45,25 45,22 47,20 50,20 53,20 55,22 55,25 55,28 53,30 50,30 Z",

        // Успех (галочка в круге)
        ToastNotificationIcon.Success => 
            "M 50,0 C 22.4,0 0,22.4 0,50 0,77.6 22.4,100 50,100 77.6,100 100,77.6 100,50 100,22.4 77.6,0 50,0 Z M 42,70 L 20,48 26,42 42,58 74,26 80,32 Z",

        // Предупреждение (треугольник с восклицательным знаком)
        ToastNotificationIcon.Warning => 
            "M 50,5 L 95,90 5,90 Z M 50,75 C 47,75 45,73 45,70 45,67 47,65 50,65 53,65 55,67 55,70 55,73 53,75 50,75 Z M 45,35 L 47,60 53,60 55,35 Z",

        // Ошибка (крестик в круге)
        ToastNotificationIcon.Error => 
            "M 50,0 C 22.4,0 0,22.4 0,50 0,77.6 22.4,100 50,100 77.6,100 100,77.6 100,50 100,22.4 77.6,0 50,0 Z M 70,65 L 65,70 50,55 35,70 30,65 45,50 30,35 35,30 50,45 65,30 70,35 55,50 Z",

        // Критическая ошибка (крестик в круге с дополнительной обводкой)
        ToastNotificationIcon.Critical => 
            "M 50,0 C 22.4,0 0,22.4 0,50 0,77.6 22.4,100 50,100 77.6,100 100,77.6 100,50 100,22.4 77.6,0 50,0 Z M 50,10 C 72.1,10 90,27.9 90,50 90,72.1 72.1,90 50,90 27.9,90 10,72.1 10,50 10,27.9 27.9,10 50,10 Z M 70,65 L 65,70 50,55 35,70 30,65 45,50 30,35 35,30 50,45 65,30 70,35 55,50 Z",

        // Вопрос (знак вопроса в круге)
        ToastNotificationIcon.Question => 
            "M 50,0 C 22.4,0 0,22.4 0,50 0,77.6 22.4,100 50,100 77.6,100 100,77.6 100,50 100,22.4 77.6,0 50,0 Z M 50,80 C 47,80 45,78 45,75 45,72 47,70 50,70 53,70 55,72 55,75 55,78 53,80 50,80 Z M 58,58 C 55,61 53,63 53,67 L 47,67 C 47,62 49,59 52,56 55,53 57,51 57,48 57,45 55,42 50,42 45,42 43,45 43,48 L 37,48 C 37,41 42,36 50,36 58,36 63,41 63,48 63,52 61,55 58,58 Z",

        _ => ""
    };
}
