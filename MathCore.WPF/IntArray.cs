using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>MarkupExtension для создания массива целых чисел из строки.</summary>
/// <example>
/// <code>
/// <![CDATA[
/// <x:Array xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
///          xmlns:sys="clr-namespace:System;assembly=mscorlib"
///          xmlns:local="clr-namespace:MathCore.WPF">
///     <local:IntArray Data="1; 2 3; 4"/>
/// </x:Array>
/// ]]>
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(int[]))]
public class IntArray : MarkupExtension
{
    /// <summary>Строка, содержащая целые числа, разделенные пробелами или точкой с запятой.</summary>
    public string Data { get; set; }

    /// <summary>Инициализирует новый экземпляр класса <see cref="IntArray"/>.</summary>
    /// <param name="Data">Строка, содержащая целые числа.</param>
    public IntArray(string Data) => this.Data = Data;

    /// <summary>Инициализирует новый экземпляр класса <see cref="IntArray"/> без начальных данных.</summary>
    public IntArray() : this(string.Empty) { }

    /// <summary>Возвращает массив целых чисел, полученных из строки <see cref="Data"/>.</summary>
    /// <param name="Services">Провайдер услуг.</param>
    /// <returns>Массив целых чисел.</returns>
    public override object ProvideValue(IServiceProvider Services)
    {
        // Разделяем строку на подстроки, используя пробел и точку с запятой в качестве разделителей.
        var data = Data.AsStringPtr().Split(';', ' ');

        // Создаём список для хранения целых чисел.
        var values = new List<int>();

        // Обрабатываем каждую подстроку.
        foreach (var str in data)
        {
            // Пытаемся преобразовать подстроку в целое число.
            if (str.TryParseInt32() is { } value)
                // Если преобразование успешно, добавляем число в список.
                values.Add(value);
        }

        // Возвращаем массив целых чисел.
        return values.ToArray();
    }
}