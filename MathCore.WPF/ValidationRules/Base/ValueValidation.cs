using System.Windows.Controls;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.WPF.ValidationRules.Base
{
    /// <summary>Правило проверки значения</summary>
    public abstract class ValueValidation : ValidationRule
    {
        /// <summary>Сообщение об ошибке</summary>
        public string? ErrorMessage { get; set; }
    }
}
