using System.Windows;
using System.Windows.Controls;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF
{
    public class ColumnDefinitionCollapsible : ColumnDefinition
    {
        static ColumnDefinitionCollapsible()
        {
            WidthProperty.OverrideMetadata(
                typeof(ColumnDefinitionCollapsible),
                new FrameworkPropertyMetadata(
                    new GridLength(1, GridUnitType.Star),
                    null,
                    (d, v) => ((ColumnDefinitionCollapsible)d).Visible ? v : new GridLength(0)));

            MinWidthProperty.OverrideMetadata(
                typeof(ColumnDefinitionCollapsible),
                new FrameworkPropertyMetadata(0d, null, (d, v) => ((ColumnDefinitionCollapsible)d).Visible ? v : 0d));
        }

        #region Visible : bool - Видимость

        /// <summary>Видимость</summary>
        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register(
                nameof(Visible),
                typeof(bool),
                typeof(ColumnDefinitionCollapsible),
                new PropertyMetadata(true, OnVisibleChanged));

        private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(WidthProperty);
            d.CoerceValue(MinWidthProperty);
        }

        /// <summary>Видимость</summary>
        public bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        #endregion
    }
}