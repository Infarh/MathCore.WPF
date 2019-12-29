using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class PathExistValidationRule : ValidationRule
    {
        public bool AllowNullString { get; set; }

        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            var path = value as string;
            var exist = (AllowNullString || !string.IsNullOrWhiteSpace(path)) 
                   && (AllowNullString && string.IsNullOrWhiteSpace(path) || Directory.Exists(path) || File.Exists(path));
            return exist ? ValidationResult.ValidResult : new ValidationResult(false, ErrorMessage ?? "Путь не существует");
        }
    }
}
