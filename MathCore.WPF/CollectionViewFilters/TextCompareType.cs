namespace MathCore.WPF;

/// <summary>Способ сравнения строк</summary>
public enum TextCompareType
{
    /// <summary>Строка должна содержать искомую строку</summary>
    Contains,
    /// <summary>Строки должны совпадать</summary>
    Equals,
    /// <summary>Строка должна начинаться с искомой последовательности</summary>
    StartWith,
    /// <summary>Строка должна заканчиваться искомой последовательностью</summary>
    EndWith
}