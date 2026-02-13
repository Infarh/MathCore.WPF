using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using MathCore.Annotations;

namespace MathCore.WPF.ViewModels;

public partial class ViewModel
{
    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="TField">Тип значения свойства</typeparam>
    /// <typeparam name="TValue">Тип значения, устанавливаемого для свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="converter">Метод преобразования значения</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<TField, TValue>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref TField? field,
        TValue? value,
        Func<TValue?, TField?> converter,
        [CallerMemberName] string PropertyName = null!) =>
        Set(ref field, converter(value), PropertyName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="ValueChecker">Метод проверки правильности устанавливаемого значения</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field,
        T? value,
        Func<T?, bool> ValueChecker,
        [CallerMemberName] string PropertyName = null!) =>
        ValueChecker(value) && Set(ref field, value, PropertyName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="TField">Тип значения свойства</typeparam>
    /// <typeparam name="TValue">Тип значения, получаемого из свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="converter">Метод преобразования значения</param>
    /// <param name="ValueChecker">Метод проверки правильности устанавливаемого значения</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<TField, TValue>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref TField? field,
        TValue? value,
        Func<TValue?, TField?> converter,
        Func<TField?, bool> ValueChecker,
        [CallerMemberName] string PropertyName = null!) =>
        Set(ref field, converter(value), ValueChecker, PropertyName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="UpdateCommandsState">Обновить состояния команд</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field,
        T? value,
        bool UpdateCommandsState,
        [CallerMemberName] string PropertyName = null!)
    {
        var result = Set(ref field, value, PropertyName);
        if (result && UpdateCommandsState)
            CommandManager.InvalidateRequerySuggested();
        return result;
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="Value">Новое значение свойства</param>
    /// <param name="OldValue">Старое значение свойства</param>
    /// <param name="Setter">Метод установки нового значения свойства</param>
    /// <param name="ValueValidator">Метод проверки возможности установки нового значения свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    protected virtual bool Set<T>(
        T? Value,
        T? OldValue,
        Action<T?> Setter,
        Func<T?, bool>? ValueValidator,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(Value, OldValue)) return false;
        if (ValueValidator is not null && !ValueValidator(Value)) return false;
        Setter(Value);
        OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary>Асинхронный метод изменения значения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Поле, хранящее значение свойства</param>
    /// <param name="value">Новое значение свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Задача, возвращающая истину, если свойство изменило своё значение</returns>
    protected virtual ValueTask<bool> SetAsync<T>([Attributes.NotNullIfNotNull(nameof(value))] ref T? field, T? value, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return new(false);
        field = value;

        return SetPropertyAsync(PropertyName);
        async ValueTask<bool> SetPropertyAsync(string property)
        {
            await Task.Factory.StartNew(s => OnPropertyChanged((string)s!), property);
            return true;
        }
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="OldValue">Предыдущее значение</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<T>([Attributes.NotNullIfNotNull(nameof(value))] ref T? field, T? value, out T? OldValue, [CallerMemberName] string PropertyName = null!)
    {
        OldValue = field;
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field,
        T? value,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="ValueChecker">Метод проверки возможности установки значения</param>
    /// <param name="OnPropertyChanged">Метод уведомления об изменении значения свойства</param>
    /// <param name="Sender">Объект-источник события</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    public static bool Set<T>(
        ref T? field,
        T? value,
        Func<T?, bool> ValueChecker,
        PropertyChangedEventHandler OnPropertyChanged,
        object? Sender,
        [CallerMemberName] string PropertyName = null!) =>
        ValueChecker(value) && Set(ref field, value, OnPropertyChanged, Sender, PropertyName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="OnPropertyChanged">Метод уведомления об изменении значения свойства</param>
    /// <param name="Sender">Объект-источник события</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    public static bool Set<T>(
        ref T? field,
        T? value,
        PropertyChangedEventHandler OnPropertyChanged,
        object? Sender,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(Sender, new(PropertyName));
        return true;
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="OnPropertyChanged">Метод уведомления об изменении значения свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    public static bool Set<T>(
        ref T? field,
        T? value,
        Action<object?, string> OnPropertyChanged,
        object? Sender, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(Sender, PropertyName);
        return true;
    }

    public static bool Set<T>(
        ref T? field,
        T? value,
        Action<string> OnPropertyChanged,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }
}
