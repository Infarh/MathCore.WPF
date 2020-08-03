using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors
{
    public interface IValidationExceptionHandler
    {
        void ValidationExceptionsChanged(int count, object ErrorContent);
    }
    
    public class ValidationExceptionBehavior : Behavior<FrameworkElement>
    {
        private int _ValidationExceptionCount;

        protected override void OnAttached() => AssociatedObject.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));

        protected override void OnDetaching() => AssociatedObject.RemoveHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(OnValidationError));

        private void OnValidationError(object? sender, ValidationErrorEventArgs e)
        {
            if (e.Error.Exception is null) return;

            if (e.Action == ValidationErrorEventAction.Added)
                _ValidationExceptionCount++;
            else
                _ValidationExceptionCount--;
            
            var view_model = AssociatedObject.DataContext as IValidationExceptionHandler;
            view_model?.ValidationExceptionsChanged(_ValidationExceptionCount, e.Error.ErrorContent);
        }
    }
}
