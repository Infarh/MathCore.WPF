using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;
using MathCore.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace MathCore.WPF.ValidationRules
{
    public class StringLength : ValidationRule
    {
        public bool AllowNull { get; set; }

        public bool AllowNotString { get; set; }

        [ConstructorArgument(nameof(Length))]
        public int Length { get; set; }

        public bool Equal { get; set; } = true;

        public bool Less { get; set; }

        public bool Gatherer { get; set; }

        public StringLength() { }

        public StringLength(int Length) => this.Length = Length;

        /// <inheritdoc />
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            var valid = ValidationResult.ValidResult;
            if (value is null) return AllowNull ? valid : new ValidationResult(false, "Значение не указано");
            if (!(value is string)) return AllowNotString ? valid : new ValidationResult(false, $"Значение {value} не является строкой");

            var str_length = ((string)value).Length;
            var len = Length;

            if (str_length == len) return Equal ? valid : new ValidationResult(false, $"Длина строки {value} не равна {len}");
            if (str_length < len) return Less ? valid : new ValidationResult(false, $"Длина строки {value} меньше чем {len}");
            if (str_length > len) return Gatherer ? valid : new ValidationResult(false, $"Длина строки {value} больше чем {len}");
            return new ValidationResult(false, $"Длина строки {value} равна {len}");
        }
    }
}