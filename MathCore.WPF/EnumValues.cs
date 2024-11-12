using System.Windows.Markup;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MathCore.WPF;

/// <summary>Класс для получения значений перечисления.</summary>
public class EnumValues : MarkupExtension
{
    /// <summary>Тип перечисления.</summary>
    private Type? _Type;

    /// <summary>Тип перечисления.</summary>
    public Type? Type
    {
        get => _Type;
        set
        {
            // Проверяем, что тип является перечислением.
            if (value is { IsEnum: false }) 
                throw new ArgumentException("Тип не является перечислением", nameof(value));
            _Type = value;
        }
    }

    /// <summary>Конструктор по умолчанию.</summary>
    public EnumValues() { }

    /// <summary>Конструктор с указанием типа перечисления.</summary>
    /// <param name="type">Тип перечисления.</param>
    public EnumValues(Type type) => Type = type;

    /// <summary>Возвращает значения перечисления.</summary>
    /// <param name="sp">Провайдер сервисов.</param>
    /// <returns>Массив значений перечисления.</returns>
    public override object? ProvideValue(IServiceProvider sp) => _Type?.GetEnumValues();
}