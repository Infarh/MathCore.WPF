using System.ComponentModel;
using System.Windows;

namespace MathCore.WPF
{
    public static class WPFService
    {
        public static bool IsInDesignMode { get; }

        static WPFService()
        {
            IsInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
                        typeof(FrameworkElement)).Metadata.DefaultValue;
        }
    }
}
