using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using MathCore.WPF.pInvoke;
// ReSharper disable InconsistentNaming
// ReSharper disable PossiblyMistakenUseOfParamsMethod

namespace MathCore.WPF;

public class GlobalHotKeyBinding : Freezable, IDisposable
{
    //(?<=<summary>)\s*\r\n\s*/{3}\s(.*)\s*\r\n\s*/{3} (?=</summary>)

    protected override Freezable CreateInstanceCore() => new GlobalHotKeyBinding();

    #region Key : Keys - Клавиша

    /// <summary>Клавиша</summary>
    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register(
            nameof(Key),
            typeof(Keys),
            typeof(GlobalHotKeyBinding),
            new(default(Keys), (d, _) => ((GlobalHotKeyBinding)d).UpdateHotkey()));

    /// <summary>Клавиша</summary>
    //[Category("")]
    [Description("Клавиша")]
    public Keys Key { get => (Keys)GetValue(KeyProperty); set => SetValue(KeyProperty, value); }

    #endregion

    #region Modifer : ModifierKeys - Модификатор клавиши

    /// <summary>Модификатор клавиши</summary>
    public static readonly DependencyProperty ModiferProperty =
        DependencyProperty.Register(
            nameof(Modifer),
            typeof(ModifierKeys),
            typeof(GlobalHotKeyBinding),
            new(ModifierKeys.None, (d, _) => ((GlobalHotKeyBinding)d).UpdateHotkey()));

    /// <summary>Модификатор клавиши</summary>
    //[Category("")]
    [Description("Модификатор клавиши")]
    public ModifierKeys Modifer { get => (ModifierKeys)GetValue(ModiferProperty); set => SetValue(ModiferProperty, value); }

    #endregion

    public (Keys Key, ModifierKeys Modifer) KeyModifer => GlobalHotKeysCollection.GetModifiers(Key, Modifer);

    #region Command : ICommand - Команда, привязываемая к горячей клавише

    /// <summary>Команда, привязываемая к горячей клавише</summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(GlobalHotKeyBinding),
            new(default(ICommand)));

    /// <summary>Команда, привязываемая к горячей клавише</summary>
    //[Category("")]
    [Description("Команда, привязываемая к горячей клавише")]
    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

    #endregion

    #region CommandParameter : object - Параметр команды

    /// <summary>Параметр команды</summary>
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(
            nameof(CommandParameter),
            typeof(object),
            typeof(GlobalHotKeyBinding),
            new(default(object)));

    /// <summary>Параметр команды</summary>
    //[Category("")]
    [Description("Параметр команды")]
    public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

    #endregion

    internal GlobalHotKeysCollection? Host { get; private set; }
    internal void SetHost(GlobalHotKeysCollection? HostCollection) => Host = HostCollection;

    private void UpdateHotkey()
    {
        CheckAccess();
        GlobalHotKeysCollection.Unregister(this);
        GlobalHotKeysCollection.Register(this);
    }

    internal ushort HotKeyId { get; set; }

    public override string ToString()
    {
        var (key, modifer) = GlobalHotKeysCollection.GetModifiers(Key, Modifer);
        return modifer == ModifierKeys.None ? key.ToString() : $"{modifer}+{key}";
    }

    public void Invoke()
    {
        Debug.WriteLine("Hot key binding {0} invoked", this);
        if(Command is not { } cmd) return;
        var parameter = CommandParameter;
        if (cmd.CanExecute(parameter))
            cmd.Execute(parameter);
    }

    public void Dispose()
    {
        var name = ToString();
        Debug.WriteLine("Hot key binding {0} disposing...", args: name);

        GlobalHotKeysCollection.Unregister(this);
        Debug.WriteLine("Hot key binding {0} disposed", args: name);
    }
}