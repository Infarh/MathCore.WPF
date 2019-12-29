using System.Globalization;
using System.IO;
using System.Windows.Controls;
using MathCore.Annotations;

namespace MathCore.WPF.ValidationRules
{
    public class FileExistValidationRule : ValidationRule
    {
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            switch (value)
            {
                default: return new ValidationResult(false, $"Unknown value type {value.GetType()}");
                case null: return new ValidationResult(false, "Null reference");
                case FileInfo file when file.Exists: return ValidationResult.ValidResult;
                case string file when File.Exists(file): return ValidationResult.ValidResult;
                case string file: return new ValidationResult(false, $"{file} not found");
            }
        }
    }
}