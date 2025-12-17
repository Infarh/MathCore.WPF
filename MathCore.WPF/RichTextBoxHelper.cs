using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>Вспомогательные методы для работы с RichTextBox и его документом</summary>
public static class RichTextBoxHelper
{
    private static readonly HashSet<Thread> __RecursionProtection = [];

    /// <summary>Получить XAML-представление документа из присоединённого свойства</summary>
    /// <param name="obj">Объект, у которого читается свойство</param>
    /// <returns>XAML документа как строка</returns>
    public static string GetDocumentXaml(DependencyObject obj) => (string)obj.GetValue(DocumentXamlProperty);

    /// <summary>Установить XAML-представление документа в присоединённое свойство</summary>
    /// <param name="obj">Объект, у которого устанавливается свойство</param>
    /// <param name="value">XAML документа или null</param>
    public static void SetDocumentXaml(DependencyObject obj, string? value)
    {
        __RecursionProtection.Add(Thread.CurrentThread);
        obj.SetValue(DocumentXamlProperty, value);
        __RecursionProtection.Remove(Thread.CurrentThread);
    }

    /// <summary>Присоединённое свойство, хранящее XAML-представление документа</summary>
    public static readonly DependencyProperty DocumentXamlProperty =
        DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof(string),
            typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata(
                "",
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (obj, _) =>
                {
                    if (__RecursionProtection.Contains(Thread.CurrentThread)) return;

                    var rich_text_box = (RichTextBox)obj;

                    // Разбор XAML в документ (или использовать XamlReader.Parse())
                    try
                    {
                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(rich_text_box)));
                        var doc    = (FlowDocument)XamlReader.Load(stream);

                        // Установить документ
                        rich_text_box.Document = doc;
                    }
                    catch (Exception)
                    {
                        rich_text_box.Document = new();
                    }

                    // При изменении документа обновлять источник
                    rich_text_box.TextChanged += (sender, _) =>
                    {
                        if (sender is not RichTextBox another_rich_text_box) return;
                        SetDocumentXaml(rich_text_box, XamlWriter.Save(another_rich_text_box.Document));
                    };
                }
            )
        );

    /// <summary>Возвращает TextRange, покрывающий слово, содержащее или следующее за данным TextPointer</summary>
    /// <remarks>
    /// Если данный TextPointer находится внутри слова или в его начале, возвращается диапазон содержащего слова
    /// Если данный TextPointer стоит между двумя словами, возвращается диапазон следующего слова
    /// Если данный TextPointer находится на границе конца слова, возвращается диапазон следующего слова
    /// </remarks>
    public static TextRange? GetWordRange(this TextPointer position)
    {
        TextRange?  word_range          = null;
        TextPointer word_start_position = null;

        // Сначала идём вперёд, чтобы найти конец слова
        var word_end_position = position.GetPositionAtWordBoundary(/*НаправлениеРазрываСлова*/LogicalDirection.Forward);

        if (word_end_position != null) // Затем идём назад, чтобы найти начало слова
            word_start_position = word_end_position.GetPositionAtWordBoundary(/*НаправлениеРазрываСлова*/ LogicalDirection.Backward);

        if (word_start_position != null && word_end_position != null)
            word_range = new(word_start_position, word_end_position);

        return word_range;
    }

    /// <summary>
    /// 1. При WordBreakDirection = Forward возвращает позицию в конце слова
    /// 2. При WordBreakDirection = Backward возвращает позицию в начале слова
    /// 3. Возвращает null, если в запрошенном направлении нет границы слова
    /// </summary>
    private static TextPointer? GetPositionAtWordBoundary(this TextPointer position, LogicalDirection WordBreakDirection)
    {
        if (!position.IsAtInsertionPosition)
            position = position.GetInsertionPosition(WordBreakDirection);

        var navigator = position;
        while (navigator != null && !navigator.IsPositionNextToWordBreak(WordBreakDirection))
            navigator = navigator.GetNextInsertionPosition(WordBreakDirection);

        return navigator;
    }

    // Вспомогательный метод для GetPositionAtWordBoundary
    // Возвращает true, если переданный TextPointer находится рядом с разделителем слова в указанном направлении
    private static bool IsPositionNextToWordBreak(this TextPointer position, LogicalDirection WordBreakDirection)
    {
        var is_at_word_boundary = false;

        // Пропустить любое форматирование
        if (position.GetPointerContext(WordBreakDirection) != TextPointerContext.Text)
            position = position.GetInsertionPosition(WordBreakDirection);

        if (position.GetPointerContext(WordBreakDirection) == TextPointerContext.Text)
        {
            var opposite_direction = (WordBreakDirection == LogicalDirection.Forward) ?
                LogicalDirection.Backward : LogicalDirection.Forward;

            var run_buffer          = new char[1];
            var opposite_run_buffer = new char[1];

            position.GetTextInRun(WordBreakDirection, run_buffer, /*startIndex*/0, /*count*/1);
            position.GetTextInRun(opposite_direction, opposite_run_buffer, /*startIndex*/0, /*count*/1);

            if (run_buffer[0] == ' ' && opposite_run_buffer[0] != ' ')
                is_at_word_boundary = true;
        }
        else
            // Если мы не рядом с текстом, считаем эту позицию границей слова
            // На практике это означает, что мы рядом с встроенным объектом или границей блока
            is_at_word_boundary = true;

        return is_at_word_boundary;
    }
}