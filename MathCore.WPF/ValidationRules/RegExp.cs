using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.ValidationRules
{
    /// <summary>Проверка на соответствие строки регулярному выражению</summary>
    public class RegExp : Base.FormattedValueValidation
    {
        public bool AllowNotString { get; set; }

        public string? NotStringErrorMessage { get; set; }

        [ConstructorArgument(nameof(Expression))]
        public string? Expression { get; set; }

        public RegExp() { }

        public RegExp(string Expression) => this.Expression = Expression;

        /// <inheritdoc />
        [NotNull]
        public override ValidationResult Validate(object value, CultureInfo c)
        {
            var valid = ValidationResult.ValidResult;
            if (value is null) 
                return AllowNull 
                    ? valid 
                    : new ValidationResult(false, NullReferenceMessage ?? ErrorMessage ?? "Значение не указано");

            if (!(value is string str)) 
                return AllowNotString 
                    ? valid
                    : new ValidationResult(false, NotStringErrorMessage ?? ErrorMessage ?? $"Значение {value} не является строкой");

            var match = Regex.Match(str, Expression);
            return match.Success 
                ? valid 
                : new ValidationResult(false, FormatErrorMessage ?? ErrorMessage ?? $"Выражение {Expression} не найдено в строке {str}");
        }
    }
}