using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors
{
    public class ListBoxItemsSelectionBinder : Behavior<ListBox>
    {
        #region SelectedItems : IList - Выбранные элементы

        /// <summary>Выбранные элементы</summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(IList),
                typeof(ListBoxItemsSelectionBinder),
                new FrameworkPropertyMetadata(default(IList), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>Выбранные элементы</summary>
        //[Category("")]
        [Description("Выбранные элементы")]
        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        #endregion

        protected override void OnAttached() => AssociatedObject.SelectionChanged += OnSelectionChanged;

        protected override void OnDetaching() => AssociatedObject.SelectionChanged -= OnSelectionChanged;

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            SelectedItems = AssociatedObject.SelectedItems;
    }
}
