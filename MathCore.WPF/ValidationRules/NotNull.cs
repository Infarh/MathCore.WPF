using System.Globalization;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules
{
    /// <summary>Проверка, что значение не является <see langword="null"/></summary>
    public class NotNull : Base.ValueValidation
    {
        /// <summary>Проверка, что значение не является <see langword="null"/></summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="c">Сведения о текущей культуре (в проверке не используется)</param>
        /// <returns>Корректный результат, если значение не является <see langword="null"/></returns>
        [NotNull]
        public override ValidationResult Validate(object? value, [CanBeNull] CultureInfo? c) => 
            value is null
                ? new ValidationResult(false, ErrorMessage ?? "Значение не указано")
                : ValidationResult.ValidResult;
    }
}