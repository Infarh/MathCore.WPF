using System.Globalization;
using System.IO;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.ValidationRules
{
    public class FileExistValidationRule : ValidationRule
    {
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c) =>
            value switch
            {
                null => new ValidationResult(false, "Null reference"),
                FileInfo file when file.Exists => ValidationResult.ValidResult,
                string file when File.Exists(file) => ValidationResult.ValidResult,
                string file => new ValidationResult(false, $"{file} not found"),
                _ => new ValidationResult(false, $"Unknown value type {value.GetType()}")
            };
    }
}