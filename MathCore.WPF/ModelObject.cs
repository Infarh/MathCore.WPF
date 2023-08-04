using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace MathCore.WPF;

/// <summary>Динамический объект доступа к свойствам модели</summary>
public sealed class ModelObject(Model Model) : DynamicObject, INotifyPropertyChanged
{
    /// <summary>Событие возникает в момент изменения значения свойства модели</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Метод генерации события изменения свойства модели</summary>
    /// <param name="PropertyName"></param>
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    /// <summary>Модель, доступ к свойствам которой осуществляет динамический объект</summary>
    private readonly Model _Model = Model.NotNull();

    /// <summary>Уведомление динамического объекта о том, что его свойство изменилось</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    internal void PropertyChangeValue(string PropertyName) => OnPropertyChanged(PropertyName);

    /// <inheritdoc />
    public override bool TryGetMember(GetMemberBinder binder, out object result) =>
        _Model.TryGetValue(binder.Name, out result) || base.TryGetMember(binder, out result);

    /// <inheritdoc />
    public override bool TrySetMember(SetMemberBinder binder, object? value) => _Model.TrySetValue(binder.Name, value) || base.TrySetMember(binder, value);
}