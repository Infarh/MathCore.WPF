using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace MathCore.WPF.ViewModels;

public partial class ViewModel
{
    /// <summary>Установить значение свойства с уведомлением об изменении</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Новое значение свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Результат установки значения</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual SetValueResult<T> SetValue<T>([Attributes.NotNullIfNotNull(nameof(value))] ref T? field, T? value, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return new(false, field, field, this);
        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new(true, old_value, value, this);
    }

    /// <summary>Установить значение свойства с проверкой и уведомлением об изменении</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Новое значение свойства</param>
    /// <param name="value_checker">Проверка возможности установки значения</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Результат установки значения</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual SetValueResult<T> SetValue<T>([Attributes.NotNullIfNotNull(nameof(value))] ref T? field, T? value, Func<T?, bool> value_checker, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value) || !value_checker(value))
            return new(false, field, value, this);
        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new(true, old_value, value, this);
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="OnPropertyChanged">Метод уведомления об изменении значения свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Результат установки значения</returns>
    public SetStaticValueResult<T> SetValue<T>(
        ref T? field,
        T? value,
        Action<string> OnPropertyChanged,
        [CallerMemberName] string PropertyName = null!)
    {
        if (OnPropertyChanged is null) throw new ArgumentNullException(nameof(OnPropertyChanged));
        if (Equals(field, value)) return new(false, field, field, OnPropertyChanged);
        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new(true, old_value, value, OnPropertyChanged);
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="ValueChecker">Метод проверки возможности установки значения</param>
    /// <param name="OnPropertyChanged">Метод уведомления об изменении значения свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Результат установки значения</returns>
    public static SetStaticValueResult<T> SetValue<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field,
        T? value,
        Func<T?, bool> ValueChecker,
        Action<string> OnPropertyChanged,
        [CallerMemberName] string PropertyName = null!)
    {
        if (OnPropertyChanged is null) throw new ArgumentNullException(nameof(OnPropertyChanged));
        if (Equals(field, value) || !ValueChecker(value)) return new(false, field, value, OnPropertyChanged);
        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new(true, old_value, value, OnPropertyChanged);
    }

    /// <summary>Установить значение свойства в словаре модели</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="value">Новое значение свойства</param>
    /// <param name="Property">Имя свойства</param>
    /// <param name="UpdateCommandsState">Обновить состояния команд</param>
    /// <returns>Истина, если значение изменилось</returns>
    protected bool Set<T>(
        T? value,
        [CallerMemberName] string Property = null!,
        bool UpdateCommandsState = false)
    {
        if (_ModelPropertyValues.TryGetValue(Property ?? throw new ArgumentNullException(nameof(Property), "Имя свойства не задано"), out var old_value) && Equals(old_value, value))
            return false;
        _ModelPropertyValues[Property] = value;
        OnPropertyChanged(Property, UpdateCommandsState);
        return true;
    }
}
