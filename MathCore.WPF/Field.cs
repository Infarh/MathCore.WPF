using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using MathCore.Annotations;

namespace MathCore.WPF
{
    /// <summary>Поле данных</summary>
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public class Field : FreezableCollection<FieldValue>, IList
    {
        #region Value

        /// <summary>Свойство зависимости, зранящее значение поля</summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(object),
                typeof(Field),
                new PropertyMetadata(default, OnValuePropertyChaged));

        private static void OnValuePropertyChaged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Field field)) return;
            foreach (var value in field)
                value.Value = e.NewValue;
        }

        /// <summary>Свойство зависимости, зранящее значение поля</summary>
        public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        #endregion

        public Field() => ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;

        private void OnCollectionChanged(object Sender, NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (FieldValue value in E.NewItems) value.Initialize(this);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (FieldValue value in E.OldItems) value.Initialize(null);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (FieldValue value in E.OldItems) value.Initialize(null);
                    foreach (FieldValue value in E.NewItems) value.Initialize(this);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var value in this) value.Initialize(this);
                    break;
            }
        }

        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            var collection = new Field();
            collection.AddItems(this.Select(value => (FieldValue)value.Clone()));
            return collection;
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FieldValue : Freezable
    {
        #region Value : object - Значение поля

        /// <summary>Значение поля</summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(object),
                typeof(FieldValue),
                new PropertyMetadata(default, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            if (!(D is FieldValue value) || value._Field is null) return;
            value._Field.Value = E.NewValue;
        }

        /// <summary>Значение поля</summary>
        public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        #endregion

        private Field _Field;

        internal void Initialize(Field field) => _Field = field;

        protected override Freezable CreateInstanceCore() => new FieldValue { _Field = _Field };
    }

    // ReSharper disable once UnusedMember.Global
}
