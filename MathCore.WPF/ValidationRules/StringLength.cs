using System.Globalization;
using System.Windows.Controls;

namespace MathCore.WPF.ValidationRules
{
    public class StringLength : ValidationRule
    {
        public bool AllowNull { get; set; }
        public bool AllowNotString { get; set; }

        public int Length { get; set; }

        public bool Equal { get; set; } = true;
        public bool Less { get; set; }
        public bool Greather { get; set; }

        public StringLength() { }
        public StringLength(int length) { Length = length; }

        /// <inheritdoc />
        public override ValidationResult Validate(object value, CultureInfo CultureInfo)
        {
            var valid = ValidationResult.ValidResult;
            if(value == null) return AllowNull ? valid : new ValidationResult(false, "Значение не указно");
            if (!(value is string)) return AllowNotString ? valid : new ValidationResult(false, $"Значение {value} не является строкой");

            var str_length = ((string) value).Length;
            var len = Length;

            if (str_length == len) return Equal ? valid : new ValidationResult(false, $"Длина строки {value} не равна {len}");
            if(str_length < len) return Less ? valid : new ValidationResult(false, $"Длина строки {value} меньше чем {len}");
            if(str_length > len) return Greather ? valid : new ValidationResult(false, $"Длина строки {value} больше чем {len}");
            return new ValidationResult(false, $"Длина строки {value} равна {len}");
        }
    }
}