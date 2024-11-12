using System.ComponentModel.DataAnnotations;
using System.Security;

namespace MathCore.WPF;

/// <summary> Атрибут, указывающий, что свойство или поле требует значения SecureString.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class RequiredSecureStringAttribute : ValidationAttribute
{
    /// <summary> Инициализирует новый экземпляр класса RequiredSecureStringAttribute.</summary>
    public RequiredSecureStringAttribute() : base("Field is required")
    {
    }

    /// <summary> Определяет, является ли значение свойства или поля SecureString допустимым.</summary>
    /// <param name="value">Значение свойства или поля.</param>
    /// <returns>true, если значение допустимо; в противном случае — false.</returns>
    public override bool IsValid(object? value) => (value as SecureString)?.Length > 0;
}