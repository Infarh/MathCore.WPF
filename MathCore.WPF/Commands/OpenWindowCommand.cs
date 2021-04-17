using System;
using System.Windows;
using System.Windows.Data;
using MathCore.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Commands
{
    public class OpenWindowCommand : LambdaCommand<Window>
    {

        private Type? _WindowType;

        public Type? WindowType
        {
            get => _WindowType;
            set
            {
                if(value is null) throw new ArgumentNullException(nameof(value));

                if (!typeof(Window).IsAssignableFrom(value))
                    throw new ArgumentException("Указанный тип не является типом окна", nameof(value));
                if (value.GetConstructor(Array.Empty<Type>()) is null)
                    throw new ArgumentException("Нельзя использовать класс окна без конструктора по умолчанию", nameof(value));
                _WindowType = value;
            }
        }

        public bool IsDialog { get; set; }

        public WindowStartupLocation StartupLocation { get; set; } = WindowStartupLocation.Manual;

        public OpenWindowCommand() { }

        public OpenWindowCommand(Type WindowType) => this.WindowType = WindowType;

        /// <inheritdoc />
        public override bool CanExecute(object? obj) => obj is Window || _WindowType != null;

        /// <inheritdoc />
        public override void Execute(object? parameter) => ShowWindow(parameter as Window ?? _WindowType?.Create<Window>() ?? throw new InvalidOperationException($"Неудалось создать окно типа {_WindowType}"));

        private void ShowWindow([NotNull] Window window)
        {
            window.SetBinding(FrameworkElement.DataContextProperty, new Binding("DataContext") { Source = TargetObject });
            if (RootObject is Window parent) window.Owner = parent;
            window.WindowStartupLocation = StartupLocation;
            if (IsDialog) window.ShowDialog(); else window.Show();
        }
    }
}