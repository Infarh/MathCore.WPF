using System.ComponentModel;
using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace MathCore.WPF.ViewModels;

public abstract record ViewModelRecord : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Генерация события изменения значения свойства</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    protected virtual void OnPropertyChanged(
        [CallerMemberName] string PropertyName = null!
        ) => 
        PropertyChanged.ThreadSafeInvoke(this, PropertyName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull("value")] ref T? field,
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
    /// <param name="OldValue">Предыдущее значение</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull("value")] ref T? field, 
        T? value, 
        out T? OldValue, 
        [CallerMemberName] string PropertyName = null!)
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
    /// <param name="ValueChecker">Метод проверки правильности устанавливаемого значения</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field,
        T? value,
        Func<T?, bool> ValueChecker,
        [CallerMemberName] string PropertyName = null!) =>
        ValueChecker(value) && Set(ref field, value, PropertyName);
}

public record TitledViewModelRecord(string? Title) : ViewModelRecord()
{
    public TitledViewModelRecord(): this(Title: null) { }

    #region Title : string? - Заголовок

    /// <summary>Заголовок</summary>
    private string? _Title = Title;

    /// <summary>Заголовок</summary>
    public string? Title { get => _Title; set => Set(ref _Title, value); }

    #endregion
}
