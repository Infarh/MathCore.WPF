using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using MathCore.WPF.pInvoke;

namespace MathCore.WPF;

/// <summary>Коллекция горячих клавиш</summary>
public class GlobalHotKeysCollection : FreezableCollection<GlobalHotKeyBinding>, IList, IDisposable
{
    /// <summary>Определение модификатора клавиши исходя из её кода</summary>
    /// <param name="Key">Код клавиши</param>
    /// <param name="Modifer">Исходный модификатор</param>
    /// <returns>Кортеж, содержаший очищенный от модификатора код клавиши и выделенный объединённый модификатор</returns>
    internal static (Keys Key, ModifierKeys Modifer) GetModifiers(Keys Key, ModifierKeys Modifer = ModifierKeys.None)
    {
        var key      = Key;
        var modifers = Modifer;

        if ((Key & Keys.Control) == Keys.Control)
        {
            modifers |= ModifierKeys.Control;
            key      ^= Keys.Control;
        }

        if ((Key & Keys.Shift) == Keys.Shift)
        {
            modifers |= ModifierKeys.Shift;
            key      ^= Keys.Shift;
        }

        if ((Key & Keys.Alt) == Keys.Alt)
        {
            modifers |= ModifierKeys.Alt;
            key      ^= Keys.Alt;
        }

        if (key is Keys.ShiftKey or Keys.ControlKey or Keys.Menu) key = Keys.None;

        return (key, modifers);
    }

    /// <summary>Инициализация новой коллекции горячих клавиш</summary>
    public GlobalHotKeysCollection() => ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;

    /// <summary>
    /// Обработчик события изменения содержимого коллекции,
    /// выполняющий подключение и отключение клавиш от внутренних обработчиков событий коллекции
    /// </summary>
    private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems?.Cast<GlobalHotKeyBinding>() is { } added)
                    foreach (var item in e.NewItems.Cast<GlobalHotKeyBinding>()) item.SetHost(this);
                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems?.Cast<GlobalHotKeyBinding>() is { } removed)
                    foreach (var item in removed) item.SetHost(null);
                break;
            case NotifyCollectionChangedAction.Replace:
                if (e.NewItems?.Cast<GlobalHotKeyBinding>() is { } @new)
                    foreach (var item in @new) item.SetHost(this);
                if (e.OldItems?.Cast<GlobalHotKeyBinding>() is { } old)
                    foreach (var item in old) item.SetHost(null);
                break;
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        var collection = new GlobalHotKeysCollection();
        //collection.AddItems(this.Select(f => (GlobalHotKeyBinding)f.Clone()));
        return collection;
    }

    /// <summary>Словарь списков объектов горячих клавиш с сопоставленными им идентификаторами</summary>
    private static Dictionary<ushort, HashSet<GlobalHotKeyBinding>>? __HotKeyBindings;

    private static Dictionary<ushort, HashSet<GlobalHotKeyBinding>> HotKeyBindings
    {
        get
        {
            if (__HotKeyBindings is null)
            {
                __HotKeyBindings                            =  [];
                ComponentDispatcher.ThreadPreprocessMessage += OnFilterMessage;
            }
            return __HotKeyBindings;
        }
    }

    /// <summary>Зарегистрировать горячую клавишу</summary>
    /// <param name="binding">Объект регистрации горячей клавиши</param>
    internal static void Register(GlobalHotKeyBinding binding)
    {
        var key_modifer = binding.KeyModifer;
        var key_id      = GetHotKeyId(key_modifer);
        binding.HotKeyId = key_id;

        HotKeyBindings.GetValueOrAddNew(key_id, () => new()).Add(binding);

        var (key, modifer) = key_modifer;
        TryRegisterHotKey(key_id, key, modifer);

        Debug.WriteLine("Hot key binding {0} registered with id {1}", binding, key_id);
    }

    /// <summary>Словарь, определяющий соответствие зарегистрированных клавиш их идентификаторам</summary>
    private static readonly Dictionary<(Keys Key, ModifierKeys Modifer), ushort> __HotKeyIds = [];

    /// <summary>Метод, позволяющий получить идентификатор клавиши</summary>
    /// <param name="Key">Определение клавиши</param>
    /// <returns>Числовой идентификатор</returns>
    private static ushort GetHotKeyId((Keys Key, ModifierKeys Modifer) Key)
    {
        if (__HotKeyIds.TryGetValue(Key, out var key_id)) return key_id;
        var (key, modifer) = Key;
        var atom_name = CreateAtomName(key, modifer);
        key_id = Kernel32.GlobalAddAtom(atom_name);
        __HotKeyIds.Add(Key, key_id);

        Debug.WriteLine("Hot key id {0} registered for {1}", key_id, Key);

        return key_id;
    }

    /// <summary>Таблица идентификаторов зарегистрированных горячих клавиш</summary>
    private static readonly HashSet<ushort> __RegisteredHotKeys = [];

    /// <summary>Зарегистрировать клавишу</summary>
    /// <param name="KeyId">Идентификатор клавиши</param>
    /// <param name="Key">Регистрируемая клавиша</param>
    /// <param name="Modifer">Модификатор клавиши</param>
    /// <returns>Истина, если клавиша успешно зарегистрирована, ложь - если клавиша занята в системе</returns>
    private static bool TryRegisterHotKey(ushort KeyId, Keys Key, ModifierKeys Modifer)
    {
        if (__RegisteredHotKeys.Contains(KeyId)) return false;

        var success = User32.RegisterHotKey(IntPtr.Zero, KeyId, Modifer, Key);

        if (!success)
        {
            User32.UnregisterHotKey(IntPtr.Zero, KeyId);

            success = User32.RegisterHotKey(IntPtr.Zero, KeyId, Modifer, Key);

            if (!success) throw new Win32Exception();
        }

        __RegisteredHotKeys.Add(KeyId);

        Debug.WriteLine("Hot key {0} registered in system at id {1}", (Modifer, Key), KeyId);

        return true;
    }

    /// <summary>Метод формирования уникального для приложения имени клавиши, зарегистрированного в системе</summary>
    /// <param name="key">Регистрируемая клавиша</param>
    /// <param name="modifer">Модификатор клавиши</param>
    /// <returns>Имя клавиши, зарегистрированное в системе</returns>
    internal static string CreateAtomName(Keys key, ModifierKeys modifer) => $"MathCore.WPF.GlobalHotKey:{modifer}+{key}";

    internal static void Unregister(GlobalHotKeyBinding binding)
    {
        var key_id       = binding.HotKeyId;
        var key_bindings = HotKeyBindings;
        if (!key_bindings.ContainsKey(key_id)) return;

        var bindings = key_bindings[key_id];

        Debug.WriteLine("Hot key binding {0} unregistered", binding);

        bindings.Remove(binding);
        if (bindings.Count == 0)
            UnregisterHotKey(key_id);
    }

    /// <summary>Отменить регистрацию клавиши</summary>
    /// <param name="KeyId">Идентификатор клавиши</param>
    private static void UnregisterHotKey(ushort KeyId)
    {
        HotKeyBindings.Remove(KeyId);
        User32.UnregisterHotKey(IntPtr.Zero, KeyId);
        __RegisteredHotKeys.Remove(KeyId);

        Kernel32.GlobalDeleteAtom(KeyId);

        Debug.WriteLine("Hot key {0} unregistered at system");
    }

    /// <summary>Метод обработки сообщений ОС о нажатии горячих клавиш</summary>
    private static void OnFilterMessage(ref MSG Msg, ref bool Handled)
    {
        if (Msg.message != (int)WM.HOTKEY || __HotKeyBindings is null) return;

        var key_id = (ushort)Msg.wParam;
        Debug.WriteLine("Hot key {0} cached for handle {1}", key_id, Msg.hwnd);

        if (!__HotKeyBindings.TryGetValue(key_id, out var bindings) || bindings is not { Count: > 0 }) return;

        foreach (var binding in bindings)
            binding.Invoke();

        Handled = true;
    }

    public void Dispose()
    {
        foreach (var binding in this)
            Unregister(binding);
    }
}