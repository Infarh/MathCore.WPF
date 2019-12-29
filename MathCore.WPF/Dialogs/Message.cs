using System;
using System.Windows;
using MathCore.WPF.Commands;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Dialogs
{
    public class Message : Dialog
    {

        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register(
                nameof(Buttons),
                typeof(MessageBoxButton),
                typeof(Message),
                new PropertyMetadata(default(MessageBoxButton)));

        public MessageBoxButton Buttons
        {
            get => (MessageBoxButton)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public static readonly DependencyProperty ImmageProperty =
            DependencyProperty.Register(
                nameof(Immage),
                typeof(MessageBoxImage),
                typeof(Message),
                new PropertyMetadata(default(MessageBoxImage)));

        public MessageBoxImage Immage
        {
            get => (MessageBoxImage)GetValue(ImmageProperty);
            set => SetValue(ImmageProperty, value);
        }

        public static readonly DependencyProperty DefaultResultProperty =
            DependencyProperty.Register(
                nameof(DefaultResult),
                typeof(MessageBoxResult),
                typeof(Message),
                new PropertyMetadata(default(MessageBoxResult)));

        public MessageBoxResult DefaultResult
        {
            get => (MessageBoxResult)GetValue(DefaultResultProperty);
            set => SetValue(DefaultResultProperty, value);
        }

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register(
                nameof(Options),
                typeof(MessageBoxOptions),
                typeof(Message),
                new PropertyMetadata(default(MessageBoxOptions)));

        public MessageBoxOptions Options
        {
            get => (MessageBoxOptions)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register(
                nameof(Result),
                typeof(MessageBoxResult),
                typeof(Message),
                new PropertyMetadata(default(MessageBoxResult)));

        public MessageBoxResult Result
        {
            get => (MessageBoxResult)GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(Message),
                new PropertyMetadata(default(string)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Message() => _OpenCommand = new LambdaCommand((Action<object?>)Open, p => !IsOpened);

        protected override void OpenDialog(object? p)
        {
            var message = p as string ?? Text;
            if(message == null) return;
            Result = MessageBox.Show(message, Title, Buttons, Immage, DefaultResult, Options);
        }
    }
}
