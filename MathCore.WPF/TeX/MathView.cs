using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MathCore.WPF.TeX
{
    public partial class MathView : FrameworkElement
    {
        #region Scale dependency property : double

        /// <summary>Маштаб</summary>
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                nameof(Scale),
                typeof(double),
                typeof(MathView),
                new PropertyMetadata(20d, OnDataChanged), v => v is null || v is double && (double)v > 0);

        /// <summary>Маштаб</summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        #endregion

        #region Foreground dependency property : Brush

        /// <summary>Кисть отрисовки текста формулы</summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                nameof(Foreground),
                typeof(Brush),
                typeof(MathView),
                new PropertyMetadata(default(Brush), OnDataChanged));

        /// <summary>Кисть отрисовки текста формулы</summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        #endregion

        #region Background dependency property : Brush

        /// <summary>Кисть отрисовки заднего фона</summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                nameof(Background),
                typeof(Brush),
                typeof(MathView),
                new PropertyMetadata(default(Brush), OnDataChanged));

        /// <summary>Кисть отрисовки заднего фона</summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion

        #region Data dependency property : string

        /// <summary>Строковое выражение</summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(string),
                typeof(MathView),
                new PropertyMetadata(default(string), OnDataChanged));

        private static void OnDataChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            var view = (MathView)D;
            var data = view.Data;
            var visual = new DrawingVisual();
            try
            {
                var formula = view._Parser.Parse(data);
                var renderer = formula.GetRenderer(TexStyle.Display, view.Scale);

                Func<Box, IEnumerable<Box>> EnumerateBoxes = null;
                EnumerateBoxes = b => b.Children.SelectMany(c => EnumerateBoxes(c)).AppendFirst(b);
                var foreground = view.Foreground;
                var background = view.Background;
                var boxes = EnumerateBoxes(renderer.Box);
                if(foreground != null)
                    boxes = boxes.ForeachLazy(b => b.Foreground = foreground);
                if(background != null)
                    boxes = boxes.ForeachLazy(b => b.Background = background);
                if(background != null || foreground != null)
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    boxes.ToArray();
                //var size = renderer.RenderSize;

                using(var dContext = visual.RenderOpen())
                    renderer.Render(dContext, 0, 1);
            } catch(Exception e)
            {
                Debug.WriteLine(e);
            }
            view.Visual = visual;
        }

        /// <summary>Строковое выражение</summary>
        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        #endregion

        private readonly TexFormulaParser _Parser = new TexFormulaParser();
        private DrawingVisual _Visual;

        protected override int VisualChildrenCount => 1;

        private DrawingVisual Visual
        {
            get { return _Visual; }
            set
            {
                RemoveVisualChild(_Visual);
                _Visual = value;
                AddVisualChild(_Visual);

                InvalidateMeasure();
                InvalidateVisual();
            }
        }

        protected override Visual GetVisualChild(int index) => _Visual;

        protected override Size MeasureOverride(Size AvailableSize) =>
            _Visual?.ContentBounds.Size ?? base.MeasureOverride(AvailableSize);
    }
}
