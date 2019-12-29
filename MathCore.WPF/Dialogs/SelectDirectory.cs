//using System;
//using System.IO;
//using System.Windows;
////using System.Windows.Forms;

//namespace MathCore.WPF.Dialogs
//{
//    public class SelectDirectory : Dialog
//    {
//        #region Dependency properties

//        #region SelectedPath property : DirectoryInfo

//        public static readonly DependencyProperty SelectedPathProperty =
//            DependencyProperty.Register(
//                nameof(SelectedPath),
//                typeof(DirectoryInfo),
//                typeof(SelectDirectory),
//                new PropertyMetadata(default(DirectoryInfo)));

//        public DirectoryInfo SelectedPath
//        {
//            get { return (DirectoryInfo)GetValue(SelectedPathProperty); }
//            set { SetValue(SelectedPathProperty, value); }
//        }

//        #endregion

//        #region ShowNewFolderButton property : bool

//        public static readonly DependencyProperty ShowNewFolderButtonProperty =
//            DependencyProperty.Register(
//                nameof(ShowNewFolderButton),
//                typeof(bool),
//                typeof(SelectDirectory),
//                new PropertyMetadata(default(bool)));

//        public bool ShowNewFolderButton
//        {
//            get { return (bool)GetValue(ShowNewFolderButtonProperty); }
//            set { SetValue(ShowNewFolderButtonProperty, value); }
//        }

//        #endregion

//        #region RootFolder property : Environment.SpecialFloder default = Desctop

//        public static readonly DependencyProperty RootFolderProperty =
//            DependencyProperty.Register(
//                nameof(RootFolder),
//                typeof(Environment.SpecialFolder),
//                typeof(SelectDirectory),
//                new PropertyMetadata(default(Environment.SpecialFolder)));

//        public Environment.SpecialFolder RootFolder
//        {
//            get { return (Environment.SpecialFolder)GetValue(RootFolderProperty); }
//            set { SetValue(RootFolderProperty, value); }
//        }

//        #endregion

//        #endregion

//        protected override void OpenDialog(object p)
//        {
//            var dialog = new FolderBrowserDialog();
//            var description = Title;
//            if (description != null)
//                dialog.Description = description;

//            var selectedpath = p as string ?? SelectedPath?.FullName;
//            if (selectedpath != null)
//                dialog.SelectedPath = selectedpath;

//            dialog.ShowNewFolderButton = ShowNewFolderButton;
//            dialog.RootFolder = RootFolder;

//            var result = dialog.ShowDialog();
//            if (result == DialogResult.OK || UpdateIfResultFalse)
//                SelectedPath = new DirectoryInfo(dialog.SelectedPath);
//        }
//    }
//}
