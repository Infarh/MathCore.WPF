using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace MathCore.WPF
{
    [MarkupExtensionReturnType(typeof(Geometry))]
    public class FontGeometry : MarkupExtension
    {
        public string? Text { get; set; }

        public FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRight;

        // ReSharper disable once StringLiteralTypo
        public FontFamily Font { get; set; } = new("Segoe UI");

        public FontStyle Style { get; set; } = FontStyles.Normal;

        public FontWeight Weight { get; set; } = FontWeights.Normal;

        public FontStretch Stretch { get; set; } = FontStretches.SemiExpanded;

        // ReSharper disable once StringLiteralTypo
        public FontFamily FallBackFontFamily { get; set; } = new("Segoe UI");
        public double Size { get; set; } = 16;

        public Point Location { get; set; } = new(0, 0);

        public FontGeometry() { }

        public FontGeometry(string text) => Text = text;

        public override object ProvideValue(IServiceProvider sp)
        {
            const double default_dip = 1.25;
            var text = new FormattedText(
                Text,
                CultureInfo.CurrentCulture,
                FlowDirection,
                new Typeface(Font, Style, Weight, Stretch, FallBackFontFamily), 
                Size, 
                Brushes.Black,
                default_dip);
            return text.BuildGeometry(Location);
        }
    }
}