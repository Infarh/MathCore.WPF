using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.ValidationRules
{
    /// <summary>Проверка существования директории</summary>
    public class DirectoryExist : Base.FormattedValueValidation
    {
        /// <summary>Метод проверяет значение на предмет того, что оно является объектом <see cref="DirectoryInfo"/>, либо строкой и путь к директории существует</summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="c">Сведения о текущей культуре</param>
        /// <returns>Результат проверки валидный, если путь к директории существует</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Проверить аргументы или открытые методы", Justification = "<Ожидание>")]
        public override ValidationResult Validate(object? value, CultureInfo c) =>
            value switch
            {
                null => AllowNull ? ValidationResult.ValidResult : new ValidationResult(false, NullReferenceMessage ?? ErrorMessage ?? "Отсутствует ссылка на объект"),
                DirectoryInfo { Exists: true } => ValidationResult.ValidResult,
                string dir when Directory.Exists(dir) => ValidationResult.ValidResult,
                string { Length: 2 } str when str[1] == ':' => new ValidationResult(false, FormatErrorMessage ?? "Некорректный формат пути"),
                string dir => new ValidationResult(false, ErrorMessage?.Contains("{0}", StringComparison.Ordinal) == true ? string.Format(c, ErrorMessage, dir) : ErrorMessage ?? $"Директория {dir} не найдена"),
                _ => new ValidationResult(false, $"Тип {value.GetType()} не поддерживается")
            };
    }
}