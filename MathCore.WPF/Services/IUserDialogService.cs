using System.IO;

namespace MathCore.WPF.Services
{
    /// <summary>Сервис диалога с пользователем</summary>
    public interface IUserDialogBase
    {
        /// <summary>Открыть диалога выбора файла для чтения</summary>
        /// <param name="Title">Заголовок диалогового окна</param>
        /// <param name="Filter">Фильтр файлов диалога</param>
        /// <param name="DefaultFilePath">Путь к файлу по умолчанию</param>
        /// <returns>Выбранный файл, либо null, если диалог был отменён</returns>
        FileInfo? OpenFile(string Title, string Filter = "Все файлы (*.*)|*.*", string? DefaultFilePath = null);

        /// <summary>Открыть диалога выбора файла для записи</summary>
        /// <param name="Title">Заголовок диалогового окна</param>
        /// <param name="Filter">Фильтр файлов диалога</param>
        /// <param name="DefaultFilePath">Путь к файлу по умолчанию</param>
        /// <returns>Выбранный файл, либо null, если диалог был отменён</returns>
        FileInfo? SaveFile(string Title, string Filter = "Все файлы (*.*)|*.*", string? DefaultFilePath = null);

        /// <summary>Диалог с текстовым вопросом и вариантами выбора Yes/No</summary>
        /// <param name="Text">Заголовок окна диалога</param>
        /// <param name="Title">Текст в окне диалога</param>
        /// <returns>Истина, если был сделан выбор Yes</returns>
        bool YesNoQuestion(string Text, string Title = "Вопрос...");

        /// <summary>Диалог с текстовым вопросом и вариантами выбора Ok/Cancel</summary>
        /// <param name="Text">Заголовок окна диалога</param>
        /// <param name="Title">Текст в окне диалога</param>
        /// <returns>Истина, если был сделан выбор Ok</returns>
        bool OkCancelQuestion(string Text, string Title = "Вопрос...");

        /// <summary>Диалог с информацией</summary>
        /// <param name="Text">Заголовок окна диалога</param>
        /// <param name="Title">Текст в окне диалога</param>
        void Information(string Text, string Title = "Вопрос...");

        /// <summary>Диалог с предупреждением</summary>
        /// <param name="Text">Заголовок окна диалога</param>
        /// <param name="Title">Текст в окне диалога</param>
        void Warning(string Text, string Title = "Вопрос...");

        /// <summary>Диалог с ошибкой</summary>
        /// <param name="Text">Заголовок окна диалога</param>
        /// <param name="Title">Текст в окне диалога</param>
        void Error(string Text, string Title = "Вопрос...");

        /// <summary>Диалог индикации прогресса операции</summary>
        /// <param name="Title">Заголовок диалога</param>
        /// <param name="Status">Текст сообщения в диалоге</param>
        /// <param name="Information">Информационное сообщение в окне диалога</param>
        /// <returns>Объект управления диалогом</returns>
        /// <example>
        /// <code>
        /// using(var dialog = dialog_service.Progress)
        /// {
        ///     await OperationAsync(dialog.Progress, dialog.Cancel);
        /// }
        /// </code>
        /// </example>
        IProgressInfo Progress(string Title, string Status, string? Information = null);

        /// <summary>Запрос ввода текста</summary>
        /// <param name="Caption">Текст в окне диалога</param>
        /// <param name="Title">Текст в заголовке окна диалога</param>
        /// <param name="Default">Текстовое значение по умолчанию</param>
        /// <returns>Введённый текст, либо null в случае отказа</returns>
        string? GetText(string Caption, string Title = "Введите текст", string? Default = "");
    }
}
