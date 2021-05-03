using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.ValidationRules
{
    /// <summary>Проверка на соответствие строки регулярному выражению</summary>
    public class RegExp : Base.FormattedValueValidation
    {
        /// <summary>Разрешить нестроковые значения (у значения будет вызван метод <see cref="object.ToString"/>)</summary>
        public bool AllowNotString { get; set; }

        /// <summary>Текст ошибки, выводимый в случае получения нестрокового значения</summary>
        public string? NotStringErrorMessage { get; set; }

        /// <summary>Регулярное выражение</summary>
        [ConstructorArgument(nameof(Expression))]
        public string? Expression { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="RegExp"/></summary>
        public RegExp() { }

        /// <summary>Инициализация нового экземпляра <see cref="RegExp"/></summary>
        /// <param name="Expression">Регулярное выражение</param>
        public RegExp([RegexPattern] string Expression) => this.Expression = Expression;

        /// <summary>Проверка - удовлетворяет ли переданное значение указанному регулярному выражению</summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="c">Сведения о текущей культуре</param>
        /// <returns>Валидный результат в случае если значение удовлетворяет указанному регулярному выражению</returns>
        [NotNull]
        public override ValidationResult Validate(object? value, CultureInfo c)
        {
            if(Expression is not { Length: > 0 } expr) return ValidationResult.ValidResult;
            var valid = ValidationResult.ValidResult;
            if (value is null) 
                return AllowNull 
                    ? valid 
                    : new ValidationResult(false, NullReferenceMessage ?? ErrorMessage ?? "Значение не указано");


            if (value is not string str) 
                return AllowNotString 
                    ? valid
                    : new ValidationResult(false, NotStringErrorMessage ?? ErrorMessage ?? $"Значение {value} не является строкой");

            var match = Regex.Match(str, expr);
            return match.Success 
                ? valid 
                : new ValidationResult(false, FormatErrorMessage ?? ErrorMessage ?? $"Выражение {expr} не найдено в строке {str}");
        }
    }
}