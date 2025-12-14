using System.ComponentModel;
using System.Windows;

namespace MathCore.WPF;

/// <summary>Сервисные свойства и методы для работы с WPF-инфраструктурой</summary>
public static class WPFService
{
    /// <summary>Признак выполнения кода в режиме конструкции</summary>
    public static bool IsInDesignMode { get; }

    static WPFService() =>
        IsInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
            typeof(FrameworkElement)).Metadata.DefaultValue;
}