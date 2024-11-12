using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>Представляет расширение разметки, которое предоставляет массив строк.</summary>
[MarkupExtensionReturnType(typeof(string[]))]
public class StringArray : MarkupExtension
{
    /// <summary> Инициализирует новый экземпляр класса <see cref="StringArray"/>.</summary>
    /// <param name="data">Начальные данные для массива строк.</param>
    public StringArray(string data) : this() => Data = data;

    /// <summary> Инициализирует новый экземпляр класса <see cref="StringArray"/> с пустой строкой данных.</summary>
    public StringArray() { }

    /// <summary> Возвращает или задает строку данных, которую необходимо разбить на массив.</summary>
    public string Data { get; set; } = string.Empty;

    /// <summary> Возвращает или задает разделитель, используемый для разбиения строки данных.</summary>
    public char Separator { get; set; }

    /// <summary> Возвращает или задает значение, указывающее, следует ли удалять пустые записи из результирующего массива.</summary>
    public bool RemoveEmpty { get; set; }

    /// <summary> Предоставляет значение расширения разметки.</summary>
    /// <param name="sp">Поставщик услуг.</param>
    /// <returns>Массив строк.</returns>
    public override object ProvideValue(IServiceProvider sp)
    {
        // Разбить строку данных на массив строк с использованием указанного разделителя и удаления пустых записей
        var result = new List<string>();
        foreach (var str in Data.AsStringPtr().Split(RemoveEmpty, Separator))
            result.Add(str);
        return result.ToArray();
    }
}