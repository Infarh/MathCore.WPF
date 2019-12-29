using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using MathCore.Annotations;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF
{
    [MarkupExtensionReturnType(typeof(Visibility))]
    public class DebugVisible : MarkupExtension
    {
        public Visibility Visibility { get; set; } = Visibility.Collapsed;
        public Visibility DebugVisibility { get; set; } = Visibility.Visible;

        public DebugVisible() { }
        public DebugVisible(Visibility DebugVisibility) => this.DebugVisibility = DebugVisibility;

        public override object ProvideValue(IServiceProvider sp) => ViewModel.IsDesignMode ? DebugVisibility : Visibility;
    }

    [MarkupExtensionReturnType(typeof(Geometry))]
    public class FontGeometry : MarkupExtension
    {
        public string Text { get; set; }

        public FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRight;

        [NotNull]
        public FontFamily Font { get; set; } = new FontFamily("Segoe UI");

        public FontStyle Style { get; set; } = FontStyles.Normal;
        public FontWeight Weight { get; set; } = FontWeights.Normal;
        public FontStretch Stretch { get; set; } = FontStretches.SemiExpanded;
        [NotNull]
        public FontFamily FallBackFontFamily { get; set; } = new FontFamily("Segoe UI");
        public double Size { get; set; } = 16;

        public Point Location { get; set; } = new Point(0, 0);

        public FontGeometry() { }
        public FontGeometry([NotNull] string text) => Text = text;

        public override object ProvideValue(IServiceProvider sp)
        {
            var text = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection, new Typeface(Font, Style, Weight, Stretch, FallBackFontFamily), Size, Brushes.Black);
            return text.BuildGeometry(Location);
        }
    }
}
