// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace MathCore.WPF.ValidationRules.Base;

/// <summary>Правило проверки значения, поддерживающего формат данных</summary>
public abstract class FormattedValueValidation : NullValueValidation
{
    /// <summary>Сообщение, выводимое в случае ошибки формата</summary>
    public string? FormatErrorMessage { get; set; }
}