using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF;

/// <summary>Создаёт привязываемое присоединённое свойство для <see cref="PasswordBox.SecurePassword"/></summary>
public static class PasswordBoxHelper
{
    /// <summary>Присоединённое свойство для привязки защищённого пароля</summary>
    // присоединённое поведение не подходит из-за неверной привязки к элементу для валидации // кратко по делу
    public static readonly DependencyProperty SecurePasswordBindingProperty = DependencyProperty
       .RegisterAttached(
            "ShadowSecurePassword",
            typeof(SecureString),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(
                new SecureString(),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                AttachedPropertyValueChanged));

    private static readonly DependencyProperty __PasswordBindingMarshallerProperty = DependencyProperty
       .RegisterAttached(
            "PasswordBindingMarshaller",
            typeof(PasswordBindingMarshaller),
            typeof(PasswordBoxHelper),
            new());

    /// <summary>Устанавливает значение защищённого пароля</summary>
    /// <param name="element">Элемент для установки значения</param>
    /// <param name="SecureString">Значение защищённого пароля</param>
    public static void SetSecurePassword(PasswordBox element, SecureString SecureString) => element.SetValue(SecurePasswordBindingProperty, SecureString);

    /// <summary>Возвращает значение защищённого пароля</summary>
    /// <param name="element">Элемент для чтения значения</param>
    /// <returns>Значение защищённого пароля</returns>
    public static SecureString GetSecurePassword(PasswordBox element) => (SecureString)element.GetValue(SecurePasswordBindingProperty)!;

    private static void AttachedPropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // требуется подписка на событие элемента для обновления значения // кратко по делу
        // обработчик обёрнут в объект присоединённого свойства ради корректной сборки мусора // кратко по делу
        // событие Unloaded использовать нельзя, оно срабатывает при переключении вкладок // кратко по делу
        var password_box = (PasswordBox)d;
        if (password_box.GetValue(__PasswordBindingMarshallerProperty) is not PasswordBindingMarshaller binding_marshaller)
        {
            binding_marshaller = new(password_box);
            password_box.SetValue(__PasswordBindingMarshallerProperty, binding_marshaller);
        }

        binding_marshaller.UpdatePasswordBox(e.NewValue as SecureString);
    }

    /// <summary>Инкапсулированная логика обработки событий</summary>
    private class PasswordBindingMarshaller
    {
        private readonly PasswordBox _PasswordBox;
        private bool _IsMarshalling;

        public PasswordBindingMarshaller(PasswordBox PasswordBox)
        {
            _PasswordBox = PasswordBox;
            _PasswordBox.PasswordChanged += PasswordBoxPasswordChanged;
        }

        public void UpdatePasswordBox(SecureString NewPassword)
        {
            if (_IsMarshalling)
                return;

            _IsMarshalling = true;
            try
            {
                // установка SecurePassword не обновляет визуальное значение, требуется Password // кратко по делу
                _PasswordBox.Password = SecureStringToString(NewPassword);

                // можно использовать копирование, но выигрыш по безопасности минимален // кратко по делу
                //newPassword.CopyInto(_passwordBox.SecurePassword);
            }
            finally
            {
                _IsMarshalling = false;
            }
        }

        private static string SecureStringToString(SecureString value)
        {
            var bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            // копирование пароля в присоединённое свойство // кратко по делу
            if (_IsMarshalling)
                return;

            _IsMarshalling = true;
            try
            {
                SetSecurePassword(_PasswordBox, _PasswordBox.SecurePassword.Copy());
            }
            finally
            {
                _IsMarshalling = false;
            }
        }
    }
}