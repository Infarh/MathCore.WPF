using System.Globalization;
using System.IO;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.ValidationRules
{
    public class DirectoryExistValidationRule : ValidationRule
    {
        [NotNull]
        public override ValidationResult Validate(object value, [CanBeNull] CultureInfo c) =>
            value switch
            {
                null => new ValidationResult(false, "Null reference"),
                DirectoryInfo dir when dir.Exists => ValidationResult.ValidResult,
                string str when str.Length == 2 && str[1] == ':' => new ValidationResult(false, "Invalid path format"),
                string dir when Directory.Exists(dir) => ValidationResult.ValidResult,
                string dir => new ValidationResult(false, $"{dir} not found"),
                _ => new ValidationResult(false, $"Unknown value type {value.GetType()}")
            };
    }
}