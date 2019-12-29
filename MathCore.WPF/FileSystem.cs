using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using MathCore.Annotations;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF
{
    /// <summary>Объект слежения за дисками, подключёнными к системе</summary>
    public sealed class FileSystem : ViewModel, IDisposable, IFileSystemViewModelFinder
    {
        /// <summary>Поле паттерна синглтон</summary>
        [CanBeNull] private static FileSystem __FileSystem;

        /// <summary>Объект слежения за дисками, подключёнными к системе</summary>
        [NotNull]
        public static FileSystem Watcher => __FileSystem ?? (__FileSystem = new FileSystem());

        /// <summary>Дескриптор окна, получающего сообщения от системы о смене состояния оборудования</summary>
        [CanBeNull] private HwndSource _WindowHandle;

        /// <summary>Кеш массива дисков системы</summary>
        [CanBeNull, ItemNotNull] private DriveInfo[] _Drives;

        /// <summary>Диски системы</summary>
        [CanBeNull, ItemNotNull] public DriveInfo[] Drives { get => _Drives; private set => SetValue(ref _Drives, value).Then(SetSystemRoots); }

        [CanBeNull, ItemNotNull] private DirectoryViewModel[] _SystemRoots;

        [CanBeNull, ItemNotNull] public DirectoryViewModel[] SystemRoots { get => _SystemRoots; set => Set(ref _SystemRoots, value); }
        private void SetSystemRoots([CanBeNull, ItemNotNull] DriveInfo[] drives) => SystemRoots = drives?.Where(d => d.IsReady).Select(d => new DirectoryViewModel(d.RootDirectory)).ToArray();

        /// <summary>Проверка списков дисков на идентичность</summary>
        /// <param name="Old">Старый список дисков</param>
        /// <param name="New">Новый список дисков</param>
        /// <returns>Истина, если списки идентичны</returns>
        private static bool IsDriveListEquals([CanBeNull, ItemNotNull] DriveInfo[] Old, [CanBeNull, ItemNotNull] DriveInfo[] New)
        {
            if (ReferenceEquals(Old, New)) return true;
            if (Old?.Length != New?.Length) return false;
            for (var i = 0; i < Old.Length; i++)
                if (Old[i].Name != New[i].Name) return false;
            return true;
        }

        /// <summary>Инициализация нового наблюдателя за дисками системы</summary>
        private FileSystem()
        {
            Drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToArray();
            var main_window = Application.Current.Windows
                .Cast<Window>()
                .FirstOrDefault(w => w.GetType().Name.Contains("MainWindow"))
                ?? Application.Current.MainWindow;
            if (main_window is null) return;
            if (main_window.IsLoaded)
                OnWindowLoaded(main_window, EventArgs.Empty);
            else
                main_window.Loaded += OnWindowLoaded;
        }

        /// <summary>обработчик события, возникающего в момент завершения загрузки окна, в котором подключается обработчик системных сообщений</summary>
        /// <param name="Sender">Окно-источник события</param>
        /// <param name="E">Аргумент события (игнорируется)</param>
        private void OnWindowLoaded([NotNull] object Sender, EventArgs E)
        {
            var window = (Window)Sender;
            _WindowHandle = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            _WindowHandle?.AddHook(WndProc);
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>Сообщение Windows, приходящее окну в момент смены конфигурации системы по поборудованию</summary>
        private const int WM_DEVICECHANGE = 537;

        /// <summary>Обработчик сообщений системы, приходящих окну приложения</summary>
        /// <param name="hwnd">Дескриптор окна, получившего сообщение</param>
        /// <param name="msg">Номер сообщения</param>
        /// <param name="wParam">Параметры сообщения</param>
        /// <param name="lParam">Параметры сообщения</param>
        /// <param name="handled">Признак того, что сообщение было обработано в оконной функции</param>
        /// <returns>Результат оброботки сообщения - должен быть <see cref="IntPtr.Zero"/></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_DEVICECHANGE:
                    Drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToArray();
                    break;
            }

            return IntPtr.Zero;
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            _WindowHandle?.Dispose();
            _WindowHandle = null;
        }

        async Task<DirectoryViewModel> IFileSystemViewModelFinder.GetModelAsync(string path)
        {
            await TaskEx.YieldAsync();
            if (!Path.IsPathRooted(path))
                path = Path.GetFullPath(path);
            var model = _SystemRoots?.FirstOrDefault(m => path.StartsWith(m.Directory.FullName, StringComparison.InvariantCultureIgnoreCase));
            if (model is null) return null;
            if (model.Directory.Name.Equals(path, StringComparison.InvariantCultureIgnoreCase)) return model;
            return await ((IFileSystemViewModelFinder)model).GetModelAsync(path);
        }
    }

    public interface IFileSystemViewModelFinder
    {
        [NotNull, ItemCanBeNull] Task<DirectoryViewModel> GetModelAsync([NotNull] string path);
    }

    public class DirectoryViewModel : ViewModel, IDisposable, IFileSystemViewModelFinder,
        IEnumerable<DirectoryViewModel>, IEnumerable<FileInfo>, IEnumerable<DirectoryInfo>,
        IEquatable<DirectoryViewModel>, IEquatable<DirectoryInfo>, IEquatable<string>
    {
        [CanBeNull] private FileSystemWatcher _Watcher;

        [NotNull] public DirectoryInfo Directory { get; }

        [CanBeNull] private bool? _CanEnumItems;

        public bool CanEnumItems => (bool)(_CanEnumItems ??= (Directory.Exists/* && Directory.CanAccessToDirectoryListItems()*/));


        [ItemNotNull] public IEnumerable<DirectoryInfo>? SubDirectories => CanEnumItems ? Directory.EnumerateDirectories() : null;


        [CanBeNull, ItemNotNull] private ObservableCollection<DirectoryViewModel> _Directories;

        [CanBeNull, ItemNotNull]
        public ThreadSaveObservableCollectionWrapper<DirectoryViewModel> Directories
        {
            get
            {
                if (!CanEnumItems) return null;
                if (_Directories != null) return _Directories.AsThreadSave();
                _Directories = new ObservableCollection<DirectoryViewModel>();
                if (!CreateWatcher())
                {
                    _CanEnumItems = false;
                    _Directories = null;
                    return null;
                }
                OnRefreshDirectoriesCommandExecutedAsync();
                return _Directories?.AsThreadSave();
            }
        }

        [CanBeNull, ItemNotNull] private ThreadSaveObservableCollectionWrapper<FileInfo> _Files;

        [ItemNotNull]
        public ThreadSaveObservableCollectionWrapper<FileInfo>? Files => _Files != null || !CanEnumItems
            ? _Files
            : _Files = new ObservableCollection<FileInfo>(Directory.EnumerateFiles()).AsThreadSave();

        [NotNull, ItemNotNull]
        public IEnumerable<FileSystemAccessRule> AccessRules
        {
            get
            {
                AuthorizationRuleCollection rules = null;
                try
                {
                    rules = Directory.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));
                }
                catch (UnauthorizedAccessException)
                {
                }

                var user = WindowsIdentity.GetCurrent();
                if (rules is null || user.Groups is null) yield break;
                foreach (var rule in rules.Cast<FileSystemAccessRule>().Where(rule => user.Groups.Contains(rule.IdentityReference)))
                    yield return rule;
            }
        }

        public bool EnableWatcher
        {
            get => _Watcher?.EnableRaisingEvents ?? false;
            set
            {
                if (_Watcher is null || _Watcher.EnableRaisingEvents == value) return;
                _Watcher.EnableRaisingEvents = value;
                OnPropertyChanged();
            }
        }

        #region Команды

        [CanBeNull] private ICommand _RefreshDirectoriesCommand;

        [CanBeNull]
        public ICommand RefreshDirectoriesCommand => _RefreshDirectoriesCommand ?? (_RefreshDirectoriesCommand = new LambdaCommand(OnRefreshDirectoriesCommandExecutedAsync, CanUpdateCommandExecuted));

        private async void OnRefreshDirectoriesCommandExecutedAsync()
        {
            if (_Directories is null) return;
            if (!CanEnumItems) throw new InvalidOperationException($"Невозможно выполнить команду обновления для дирректории {Directory}: отсутствует право на доступ для извлечения содержимого дирректории");
            var dirs = _Directories;
            var sub_dirs = SubDirectories ?? throw new InvalidOperationException($"Невозможно выполнить команду обновления для дирректории {Directory}: отсутствует право на доступ для извлечения содержимого дирректории");

            await TaskEx.YieldAsync();

            dirs.Select(d => d.Directory.FullName).Xor(sub_dirs.Select(d => d.FullName),
                out var to_remove, out var to_add,
                out _, out _, out _, out _);
            foreach (var path in to_remove)
            {
                var dir = dirs.FirstOrDefault(d => string.Equals(d.Directory.FullName, path, StringComparison.InvariantCultureIgnoreCase));
                if (dir is null) continue;
                dirs.Remove(dir);
            }
            to_add.Select(path => new DirectoryViewModel(path)).AddTo(dirs);
        }

        [CanBeNull] private ICommand _RefreshFilesCommand;

        [CanBeNull] public ICommand RefreshFilesCommand => _RefreshFilesCommand ?? (_RefreshFilesCommand = new LambdaCommand(OnRefreshFilesCommandExecuted, CanUpdateCommandExecuted));

        private void OnRefreshFilesCommandExecuted()
        {
            var files = Files ?? throw new InvalidOperationException($"Невозможно выполнить команду обновления для дирректории {Directory}: отсутствует право на доступ для извлечения содержимого дирректории");
            files.Select(f => f.FullName).Xor(Directory.EnumerateFiles().Select(f => f.FullName), out var to_remove, out var to_add, out _, out _, out _, out _);
            foreach (var path in to_remove)
            {
                var file = files.FirstOrDefault(f => string.Equals(f.FullName, path, StringComparison.InvariantCultureIgnoreCase));
                if (file is null) continue;
                files.Remove(file);
            }
            to_add.Select(path => new FileInfo(path)).AddTo(files);
        }

        [CanBeNull] private ICommand _RefreshCommand;

        [CanBeNull] public ICommand RefreshCommand => _RefreshCommand ?? (_RefreshCommand = new LambdaCommand(OnRefreshCommandExecuted, CanUpdateCommandExecuted));

        private void OnRefreshCommandExecuted()
        {
            OnRefreshDirectoriesCommandExecutedAsync();
            OnRefreshFilesCommandExecuted();
        }

        private bool CanUpdateCommandExecuted() => CanEnumItems;

        #endregion

        public DirectoryViewModel([NotNull] string path) : this(new DirectoryInfo(path ?? throw new ArgumentNullException(nameof(path)))) { }

        public DirectoryViewModel([NotNull] DirectoryInfo directory) => Directory = directory ?? throw new ArgumentNullException(nameof(directory));

        private bool CreateWatcher()
        {
            if (!Directory.Exists) return false;
            try
            {
                var watcher = new FileSystemWatcher(Directory.FullName) { EnableRaisingEvents = true };
                watcher.Created += OnDirectoryChanged;
                watcher.Renamed += OnDirectoryChanged;
                watcher.Deleted += OnDirectoryChanged;
                _Watcher = watcher;
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private void OnDirectoryChanged([CanBeNull] object Sender, [NotNull] FileSystemEventArgs E)
        {
            var path = E.FullPath;
            switch (E.ChangeType)
            {
                case WatcherChangeTypes.Renamed:
                    if (File.Exists(path))
                    {
                        if (_Files is null) break;
                        foreach (var old_file in _Files.Where(f => !f.Exists).ToArray())
                            _Files.Remove(old_file);
                        _Files.Add(new FileInfo(path));
                    }
                    else if (System.IO.Directory.Exists(path))
                    {
                        if (_Directories is null) break;
                        foreach (var old_dir in _Directories.Where(d => !System.IO.Directory.Exists(d.Directory.FullName)).ToArray())
                            _Directories.Remove(old_dir);
                        _Directories.Add(new DirectoryViewModel(path));
                        OnPropertyChanged(nameof(SubDirectories));
                    }
                    break;
                case WatcherChangeTypes.Created:
                    if (File.Exists(path))
                        _Files?.Add(new FileInfo(path));
                    else if (System.IO.Directory.Exists(path))
                    {
                        if (_Directories is null) break;
                        _Directories.Add(new DirectoryViewModel(path));
                        OnPropertyChanged(nameof(SubDirectories));
                    }
                    break;
                case WatcherChangeTypes.Deleted:
                    var file = Files?.FirstOrDefault(f => string.Equals(f.FullName, path, StringComparison.InvariantCultureIgnoreCase));
                    if (file != null)
                    {
                        Files.Remove(file);
                        break;
                    }

                    var dir = Directories?.FirstOrDefault(d => string.Equals(d.Directory.FullName, path, StringComparison.InvariantCultureIgnoreCase));
                    if (dir != null)
                    {
                        Directories.Remove(dir);
                        OnPropertyChanged(nameof(SubDirectories));
                    }

                    break;
            }
        }

        #region IEquatable

        public bool Equals([ItemNotNull] DirectoryViewModel model) => ReferenceEquals(this, model) || string.Equals(Directory.FullName, model?.Directory.FullName, StringComparison.InvariantCultureIgnoreCase);

        public bool Equals(DirectoryInfo dir) => ReferenceEquals(Directory, dir) || string.Equals(Directory.FullName, dir?.FullName, StringComparison.InvariantCultureIgnoreCase);

        public bool Equals(string path) => string.Equals(Directory.FullName.TrimEnd('\\', '/'), path?.TrimEnd('\\', '/'), StringComparison.InvariantCultureIgnoreCase);

        #endregion

        public override string ToString() => $"View model:{Directory.FullName}";

        public override int GetHashCode() => Directory.GetHashCode();

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                default: return false;
                case DirectoryViewModel model: return Equals(model);
                case DirectoryInfo dir: return Equals(dir);
                case string path: return Equals(path);
            }
        }

        /// <inheritdoc />
        void IDisposable.Dispose() => _Watcher?.Dispose();

        IEnumerator<DirectoryInfo> IEnumerable<DirectoryInfo>.GetEnumerator() => (SubDirectories ?? Enumerable.Empty<DirectoryInfo>()).GetEnumerator();

        public IEnumerator<DirectoryViewModel> GetEnumerator() => (Directories ?? Enumerable.Empty<DirectoryViewModel>()).GetEnumerator();

        IEnumerator<FileInfo> IEnumerable<FileInfo>.GetEnumerator() => (Files ?? Enumerable.Empty<FileInfo>()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<DirectoryViewModel>)this).GetEnumerator();

        async Task<DirectoryViewModel> IFileSystemViewModelFinder.GetModelAsync(string DirectoryName)
        {
            await TaskEx.YieldAsync();
            if (!CanEnumItems) return null;
            if (_Directories != null && _Directories.Count > 0)
                return _Directories.FirstOrDefault(m => string.Equals(m.Directory.Name, DirectoryName, StringComparison.InvariantCultureIgnoreCase));
            var dirs = _Directories = new ObservableCollection<DirectoryViewModel>();
            if (!CreateWatcher())
            {
                _CanEnumItems = false;
                _Directories = null;
                return null;
            }
            var sub_dirs = SubDirectories ?? throw new InvalidOperationException($"Невозможно выполнить команду обновления для дирректории {Directory}: отсутствует право на доступ для извлечения содержимого дирректории");
            DirectoryViewModel model = null;
            foreach (var dir in sub_dirs)
            {
                var m = new DirectoryViewModel(dir);
                dirs.Add(m);
                if (model is null && dir.FullName.StartsWith(DirectoryName, StringComparison.InvariantCultureIgnoreCase))
                    model = m;
            }

            if (model is null) return null;
            if (model.Directory.FullName.Equals(DirectoryName, StringComparison.InvariantCultureIgnoreCase))
                return model;
            return await ((IFileSystemViewModelFinder)model).GetModelAsync(DirectoryName);
        }
    }
}
