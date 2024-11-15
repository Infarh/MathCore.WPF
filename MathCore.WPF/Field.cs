﻿using System.Collections.Specialized;
using System.Windows;

namespace MathCore.WPF;

/// <summary>Поле данных</summary>
public class Field : FreezableCollection<FieldValue>
{
    #region Value

    /// <summary>Свойство зависимости, хранящее значение поля</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(Field),
            new(default, OnValuePropertyChanged));

    private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Field field) return;
        foreach (var value in field)
            value.Value = e.NewValue;
    }

    /// <summary>Свойство зависимости, хранящее значение поля</summary>
    public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    #endregion

    /// <summary>Инициализация нового поля</summary>
    public Field() => ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;

    /// <summary>При изменении поля</summary>
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

// ReSharper disable once UnusedMember.Global