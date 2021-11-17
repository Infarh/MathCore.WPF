using System;
using System.Runtime.InteropServices;

namespace MathCore.WPF.pInvoke
{
    public static class Shell32
    {
        private const string FileName = "shell32.dll";

        [DllImport(FileName, SetLastError = true)]
        public static extern IntPtr SHAppBarMessage(AppBarMessage dwMessage, [In] ref AppBarData pData);
    }
}
