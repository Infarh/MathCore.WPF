namespace MathCore.WPF.TeX;

/// <summary>
/// Утилиты и константы для работы с TeX механизмом рендеринга
/// </summary>
internal static class TexUtilities
{
    /// <summary>Пространство имён для ресурсов стилей TeX</summary>
    public const string ResourcesStylesNamespace = "MathCore.WPF.TeX.Styles.";

    /// <summary>Пространство имён для ресурсов шрифтов TeX</summary>
    public const string ResourcesFontsNamespace = "MathCore.WPF.TeX.Fonts.";

    /// <summary>Точность сравнения чисел с плавающей запятой при вычислении метрик шрифтов</summary>
    public const double FloatPrecision = 0.0000001;
}