using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MathCore.Annotations;

namespace MathCore.WPF.ViewModels;

/// <summary>Представляет базовый класс для зависимых объектов, реализующих интерфейс INotifyPropertyChanged.</summary>
public abstract class DependencyViewModel : Freezable, INotifyPropertyChanged
{
    /// <summary>Возникает, когда значение свойства изменяется.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Уведомляет об изменении значения свойства.</summary>
    /// <param name="PropertyName">Имя свойства, значение которого изменилось. Если не указано, имя определяется автоматически.</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? PropertyName = null) => PropertyChanged?.Invoke(this, new(PropertyName));
}