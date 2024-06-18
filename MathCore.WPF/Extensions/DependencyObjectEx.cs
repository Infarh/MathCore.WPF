using System.Windows;
using System.Windows.Data;

namespace MathCore.WPF.Extensions;

public static class DependencyObjectEx
{
    public static Binding? BindingGet(this DependencyObject obj, DependencyProperty property) => BindingOperations
        .GetBinding(obj.NotNull(), property.NotNull());

    public static BindingBase? BindingGetBase(this DependencyObject obj, DependencyProperty property) => BindingOperations
        .GetBindingBase(obj.NotNull(), property.NotNull());

    public static BindingExpression? BindingGetExpression(this DependencyObject obj, DependencyProperty property) => BindingOperations
        .GetBindingExpression(obj.NotNull(), property.NotNull());

    public static BindingExpressionBase? BindingGetExpressionBase(this DependencyObject obj, DependencyProperty property) => BindingOperations
        .GetBindingExpressionBase(obj.NotNull(), property.NotNull());

    public static void BindingClearAll(this DependencyObject obj) => BindingOperations.ClearAllBindings(obj.NotNull());

    public static void BindingClear(this DependencyObject obj, DependencyProperty property) => BindingOperations
        .ClearBinding(obj.NotNull(), property.NotNull());

    public static void BindingUpdateSource(this DependencyObject obj, DependencyProperty property) => obj
        .BindingGetExpression(property)?
        .UpdateSource();

    public static void BindingUpdateTarget(this DependencyObject obj, DependencyProperty property) => obj
        .BindingGetExpression(property)?
        .UpdateTarget();

    public static void BindingValidateWithoutUpdate(this DependencyObject obj, DependencyProperty property) => obj
        .BindingGetExpression(property)?
        .ValidateWithoutUpdate();
}
