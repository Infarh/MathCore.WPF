using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace MathCore.WPF.pInvoke
{
    internal static class Kernel32
    {
        private const string FileName = "kernel32.dll";

        [DllImport(FileName, SetLastError = true)]
        public static extern ushort GlobalAddAtom(string name);

        [DllImport(FileName, SetLastError = true)]
        public static extern ushort GlobalDeleteAtom(ushort atom);
    }
}