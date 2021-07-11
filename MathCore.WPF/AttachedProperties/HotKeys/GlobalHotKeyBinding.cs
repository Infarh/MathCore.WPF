using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using MathCore.WPF.pInvoke;
// ReSharper disable InconsistentNaming

namespace MathCore.WPF
{
    public class GlobalHotKeyBinding : Freezable, IDisposable
    {
        //(?<=<summary>)\s*\r\n\s*/{3}\s(.*)\s*\r\n\s*/{3} (?=</summary>)

        private static (Keys Key, ModifierKeys Modifer) GetModifiers(Keys Key, ModifierKeys Modifer = ModifierKeys.None)
        {
            var key = Key;
            var modifers = Modifer;

            if ((Key & Keys.Control) == Keys.Control)
            {
                modifers |= ModifierKeys.Control;
                key ^= Keys.Control;
            }

            if ((Key & Keys.Shift) == Keys.Shift)
            {
                modifers |= ModifierKeys.Shift;
                key ^= Keys.Shift;
            }

            if ((Key & Keys.Alt) == Keys.Alt)
            {
                modifers |= ModifierKeys.Alt;
                key ^= Keys.Alt;
            }

            if (key is Keys.ShiftKey or Keys.ControlKey or Keys.Menu) key = Keys.None;

            return (key, modifers);
        }

        protected override Freezable CreateInstanceCore() => new GlobalHotKeyBinding();

        #region Key : Keys - Клавиша

        /// <summary>Клавиша</summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(
                nameof(Key),
                typeof(Keys),
                typeof(GlobalHotKeyBinding),
                new PropertyMetadata(default(Keys), (d, _) => ((GlobalHotKeyBinding)d).UpdateHotkey()));

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
                new PropertyMetadata(ModifierKeys.None, (d, _) => ((GlobalHotKeyBinding)d).UpdateHotkey()));

        /// <summary>Модификатор клавиши</summary>
        //[Category("")]
        [Description("Модификатор клавиши")]
        public ModifierKeys Modifer { get => (ModifierKeys)GetValue(ModiferProperty); set => SetValue(ModiferProperty, value); }

        #endregion

        #region Command : ICommand - Команда, привязываемая к горячей клавише

        /// <summary>Команда, привязываемая к горячей клавише</summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(GlobalHotKeyBinding),
                new PropertyMetadata(default(ICommand)));

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
                new PropertyMetadata(default(object)));

        private IDisposable _DisposableImplementation;

        /// <summary>Параметр команды</summary>
        //[Category("")]
        [Description("Параметр команды")]
        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        #endregion

        private IntPtr _WindowHandle;

        public void SetHost(Window? HostWindow)
        {
            if (HostWindow is null) _WindowHandle = IntPtr.Zero;
            else if (HostWindow.IsInitialized)
            {
                _WindowHandle = HostWindow.GetWindowHandle();
                UpdateHotkey();
            }
            else HostWindow.SourceInitialized += OnWindowInitialized;
        }

        private void OnWindowInitialized(object? Sender, EventArgs EventArgs)
        {
            var window = (Window)Sender!;
            window.SourceInitialized -= OnWindowInitialized;
            _WindowHandle = window.GetWindowHandle();
            UpdateHotkey();
        }

        private void UpdateHotkey()
        {
            ResetHotKey();
            SetHotKey(Key, Modifer, _WindowHandle);
        }

        private ushort _HotKeyAtomId;
        private string _HotKeyAtom;

        private void ResetHotKey()
        {
            if (_HotKeyAtomId == 0) return;
            User32.UnregisterHotKey(IntPtr.Zero, _HotKeyAtomId);
            Kernel32.GlobalDeleteAtom(_HotKeyAtomId);
            _HotKeyAtomId = 0;
            ComponentDispatcher.ThreadPreprocessMessage -= OnFilterMessage;
        }

        private void SetHotKey(Keys key, ModifierKeys modifer, IntPtr hWnd)
        {
            (key, modifer) = GetModifiers(key, modifer);
            if (hWnd == IntPtr.Zero || key == Keys.None) return;

            var atom_name = CreateAtomName(key, modifer);
            _HotKeyAtomId = Kernel32.GlobalAddAtom(_HotKeyAtom = atom_name);

            var is_registred = User32.RegisterHotKey(hWnd, _HotKeyAtomId, modifer, key);

            if (!is_registred)
            {
                User32.UnregisterHotKey(IntPtr.Zero, _HotKeyAtomId);

                is_registred = User32.RegisterHotKey(hWnd, _HotKeyAtomId, modifer, key);

                if (!is_registred) throw new Win32Exception();
            }

            ComponentDispatcher.ThreadPreprocessMessage += OnFilterMessage;
        }

        private string KeyString
        {
            get
            {
                var (key, modifer) = GetModifiers(Key, Modifer);
                return modifer == ModifierKeys.None ? key.ToString() : $"{modifer}+{key}";
            }
        }

        private static string CreateAtomName(Keys key, ModifierKeys modifer) => $"MathCore.WPF.GlobalHotKey:{modifer}+{key}";

        private void OnFilterMessage(ref MSG Msg, ref bool Handled)
        {
            if (Msg.message != (int)WM.HOTKEY || Msg.hwnd != _WindowHandle || Msg.wParam != (IntPtr)_HotKeyAtomId || Command is not { } cmd) return;
            var parameter = CommandParameter;
            if (cmd.CanExecute(parameter))
                cmd.Execute(parameter);

            Handled = true;
        }

        public void Dispose() => ResetHotKey();
    }
}