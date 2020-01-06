using System.Globalization;
using System.IO;
using System.Windows.Controls;

using MathCore.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules
{
    public class PathExistValidationRule : ValidationRule
    {
        public bool AllowNullString { get; set; }

        public string? ErrorMessage { get; set; }

        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            var path = value as string;
            var exist = (AllowNullString || !string.IsNullOrWhiteSpace(path))
                   && (AllowNullString && string.IsNullOrWhiteSpace(path) || Directory.Exists(path) || File.Exists(path));
            return exist ? ValidationResult.ValidResult : new ValidationResult(false, ErrorMessage ?? "Путь не существует");
        }
    }
}