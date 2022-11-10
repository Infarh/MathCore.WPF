using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Windows.Markup;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestWindow2ViewModel))]
internal class TestWindow2ViewModel : ViewModel
{
    #region Title : string - Заголовок окна

    /// <summary>Заголовок окна</summary>
    private string _Title = "Тестовое окно!!!";

    /// <summary>Заголовок окна</summary>
    public string Title { get => _Title; set => Set(ref _Title, value); }

    #endregion

    #region Registration : UserPasswordViewModel - Форма входа

    /// <summary>Форма входа</summary>
    private UserPasswordViewModel _Registration = new();

    /// <summary>Форма входа</summary>
    public UserPasswordViewModel Registration { get => _Registration; set => Set(ref _Registration, value, v => v != null); }

    #endregion
}

[MarkupExtensionReturnType(typeof(UserPasswordViewModel))]
internal class UserPasswordViewModel : ValidableViewModel
{
    #region UserName : string - Имя пользователя

    /// <summary>Имя пользователя</summary>
    private string _UserName;

    /// <summary>Имя пользователя</summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Не указано имя пользователя")]
    [MinLength(3, ErrorMessage = "Минимальная длина имени 3 символа")] 
    [MaxLength(10, ErrorMessage = "Максимальная длина имени 10 символов")]
    [RegularExpression("[A-Za-z][A-Za-z0-9_-]+", ErrorMessage = "Неверный формат имени")]
    public string UserName { get => _UserName; set => Set(ref _UserName, value); }

    #endregion

    #region Password : SecureString? - Пароль

    /// <summary>Пароль</summary>
    private SecureString? _Password;

    /// <summary>Пароль</summary>
    public SecureString? Password { get => _Password; set => Set(ref _Password, value); }

    [DependencyOn(nameof(Password))]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Не указан пароль")]
    public string? PasswordStr => Password?.ToString();

    #endregion
}