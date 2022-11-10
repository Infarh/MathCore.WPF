// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace MathCore.WPF.ValidationRules.Base;

/// <summary>Правило проверки значения, поддерживающего <see langword="null"/></summary>
public abstract class NullValueValidation : ValueValidation
{
    /// <summary>Разрешить пустую ссылку?</summary>
    public bool AllowNull { get; set; }

    /// <summary>Сообщение, выводимое в случае обнаружения <see langword="null"/> если <see cref="AllowNull"/> == <see langword="false"/>.</summary>
    public string? NullReferenceMessage { get; set; }
}