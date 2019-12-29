using System.Globalization;
using System.IO;
using System.Windows.Controls;
using MathCore.Annotations;

namespace MathCore.WPF.ValidationRules
{
    public class DirectoryExistValidationRule : ValidationRule
    {
        [NotNull]
        public override ValidationResult Validate(object value, [CanBeNull] CultureInfo c)
        {
            switch (value)
            {
                default: return new ValidationResult(false, $"Unknown value type {value.GetType()}");
                case null: return new ValidationResult(false, "Null reference");
                case DirectoryInfo dir when dir.Exists: return ValidationResult.ValidResult;
                case string str when str.Length == 2 && str[1] == ':': return new ValidationResult(false, "Invalid path format");
                case string dir when Directory.Exists(dir): return ValidationResult.ValidResult;
                case string dir: return new ValidationResult(false, $"{dir} not found");
            }
        }
    }
}