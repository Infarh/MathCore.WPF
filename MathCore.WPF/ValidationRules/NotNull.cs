using System.Globalization;
using System.Windows.Controls;

using MathCore.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules
{
    public class NotNull : ValidationRule
    {
        public string? ErrorMessage { get; set; }

        /// <inheritdoc />
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo с) => value is null
            ? new ValidationResult(false, ErrorMessage ?? "Значение не указано")
            : ValidationResult.ValidResult;
    }
}