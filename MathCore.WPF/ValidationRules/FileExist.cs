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
    /// <summary>Проверка существования файла</summary>
    public class FileExist : Base.FormattedValueValidation
    {
        /// <summary>Метод проверяет значение на предмет того, что оно является объектом <see cref="FileInfo"/>, либо строкой и путь к файлу существует</summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="c">Сведения о текущей культуре</param>
        /// <returns>Результат проверки валидный, если путь к файлу существует</returns>
        [NotNull]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Проверить аргументы или открытые методы", Justification = "<Ожидание>")]
        public override ValidationResult Validate(object? value, CultureInfo c) =>
            value switch
            {
                null => AllowNull ? ValidationResult.ValidResult : new ValidationResult(false, NullReferenceMessage ?? ErrorMessage ?? "Отсутствует ссылка на объект"),
                FileInfo file when file.Exists => ValidationResult.ValidResult,
                string file when File.Exists(file) => ValidationResult.ValidResult,
                string file => new ValidationResult(false, ErrorMessage?.Contains("{0}", StringComparison.Ordinal) == true ? string.Format(c, ErrorMessage, file) : ErrorMessage ?? $"Файл {file} не найден"),
                _ => new ValidationResult(false, $"Тип {value.GetType()} не поддерживается")
            };
    }
}