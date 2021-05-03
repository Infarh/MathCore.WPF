using System.Globalization;
using System.IO;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.ValidationRules
{
    /// <summary>Проверка существования пути в файловой системе</summary>
    public class PathExist : Base.NullValueValidation
    {
        /// <summary>Проверка, что путь в файловой системе существует</summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="c">Сведения о текущей культуре</param>
        /// <returns>Результат проверки валидный, если путь в файловой системе существует</returns>
        [NotNull]
        public override ValidationResult Validate(object? value, CultureInfo c)
        {
            var path = value as string;
            var exist = (AllowNull || !string.IsNullOrWhiteSpace(path))
                   && (AllowNull && string.IsNullOrWhiteSpace(path) || Directory.Exists(path) || File.Exists(path));
            return exist 
                ? ValidationResult.ValidResult 
                : new ValidationResult(false, ErrorMessage ?? "Путь не существует");
        }
    }
}