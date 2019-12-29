using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using MathCore.Annotations;

namespace MathCore.WPF
{
    public static class RichTextBoxHelper
    {
        private static readonly HashSet<Thread> __RecursionProtection = new HashSet<Thread>();

        [CanBeNull] public static string GetDocumentXaml([NotNull] DependencyObject obj) => (string)obj.GetValue(DocumentXamlProperty);

        public static void SetDocumentXaml([NotNull] DependencyObject obj, [CanBeNull] string value)
        {
            __RecursionProtection.Add(Thread.CurrentThread);
            obj.SetValue(DocumentXamlProperty, value);
            __RecursionProtection.Remove(Thread.CurrentThread);
        }

        [NotNull]
        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached(
                "DocumentXaml",
                typeof(string),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata(
                    "",
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (obj, e) =>
                    {
                        if (__RecursionProtection.Contains(Thread.CurrentThread)) return;

                        var rich_text_box = (RichTextBox)obj;

                        // Parse the XAML to a document (or use XamlReader.Parse())
                        try
                        {
                            var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(rich_text_box)));
                            var doc = (FlowDocument)XamlReader.Load(stream);

                            // Set the document
                            rich_text_box.Document = doc;
                        }
                        catch (Exception)
                        {
                            rich_text_box.Document = new FlowDocument();
                        }

                        // When the document changes update the source
                        rich_text_box.TextChanged += (sender, args) =>
                        {
                            if (!(sender is RichTextBox another_rich_text_box)) return;
                            SetDocumentXaml(rich_text_box, XamlWriter.Save(another_rich_text_box.Document));
                        };
                    }
                )
            );

        /// <summary>
        /// Returns a TextRange covering a word containing or following this TextPointer.
        /// </summary>
        /// <remarks>
        /// If this TextPointer is within a word or at start of word, the containing word range is returned.
        /// If this TextPointer is between two words, the following word range is returned.
        /// If this TextPointer is at trailing word boundary, the following word range is returned.
        /// </remarks>
        [NotNull]
        public static TextRange GetWordRange([NotNull] this TextPointer position)
        {
            TextRange wordRange = null;
            TextPointer wordStartPosition = null;

            // Go forward first, to find word end position.
            var wordEndPosition = position.GetPositionAtWordBoundary(/*WordBreakDirection*/LogicalDirection.Forward);

            if (wordEndPosition != null) // Then travel backwards, to find word start position.
                wordStartPosition = wordEndPosition.GetPositionAtWordBoundary(/*WordBreakDirection*/ LogicalDirection.Backward);

            if (wordStartPosition != null && wordEndPosition != null)
                wordRange = new TextRange(wordStartPosition, wordEndPosition);

            return wordRange;
        }

        /// <summary>
        /// 1.  When WordBreakDirection = Forward, returns a position at the end of the word,
        ///     i.e. a position with a wordBreak character (space) following it.
        /// 2.  When WordBreakDirection = Backward, returns a position at the start of the word,
        ///     i.e. a position with a wordBreak character (space) preceeding it.
        /// 3.  Returns null when there is no workbreak in the requested direction.
        /// </summary>
        [CanBeNull]
        private static TextPointer GetPositionAtWordBoundary([NotNull] this TextPointer position, LogicalDirection WordBreakDirection)
        {
            if (!position.IsAtInsertionPosition)
                position = position.GetInsertionPosition(WordBreakDirection);

            var navigator = position;
            while (navigator != null && !navigator.IsPositionNextToWordBreak(WordBreakDirection))
                navigator = navigator.GetNextInsertionPosition(WordBreakDirection);

            return navigator;
        }

        // Helper for GetPositionAtWordBoundary.
        // Returns true when passed TextPointer is next to a wordBreak in requested direction.
        private static bool IsPositionNextToWordBreak([NotNull] this TextPointer position, LogicalDirection WordBreakDirection)
        {
            var is_at_word_boundary = false;

            // Skip over any formatting.
            if (position.GetPointerContext(WordBreakDirection) != TextPointerContext.Text)
                position = position.GetInsertionPosition(WordBreakDirection);

            if (position.GetPointerContext(WordBreakDirection) == TextPointerContext.Text)
            {
                var opposite_direction = (WordBreakDirection == LogicalDirection.Forward) ?
                    LogicalDirection.Backward : LogicalDirection.Forward;

                var run_buffer = new char[1];
                var opposite_run_buffer = new char[1];

                position.GetTextInRun(WordBreakDirection, run_buffer, /*startIndex*/0, /*count*/1);
                position.GetTextInRun(opposite_direction, opposite_run_buffer, /*startIndex*/0, /*count*/1);

                if (run_buffer[0] == ' ' && opposite_run_buffer[0] != ' ')
                    is_at_word_boundary = true;
            }
            else
            {
                // If we're not adjacent to text then we always want to consider this position a "word break". 
                // In practice, we're most likely next to an embedded object or a block boundary.
                is_at_word_boundary = true;
            }

            return is_at_word_boundary;
        }
    }
}
