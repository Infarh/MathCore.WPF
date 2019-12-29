using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;

namespace MathCore.WPF.pInvoke
{
    [StructLayout(LayoutKind.Sequential, Pack = 0), SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct RECT
    {
        /// <summary> Win32 </summary>
        public int left;
        /// <summary> Win32 </summary>
        public int top;
        /// <summary> Win32 </summary>
        public int right;
        /// <summary> Win32 </summary>
        public int bottom;

        /// <summary> Win32 </summary>
        public static readonly RECT Empty = new RECT();

        /// <summary> Win32 </summary>
        public int Width => Math.Abs(right - left);  // Abs needed for BIDI OS

        /// <summary> Win32 </summary>
        public int Height => bottom - top;

        /// <summary> Win32 </summary>
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }


        /// <summary> Win32 </summary>
        public RECT(RECT rcSrc)
        {
            left = rcSrc.left;
            top = rcSrc.top;
            right = rcSrc.right;
            bottom = rcSrc.bottom;
        }

        /// <summary> Win32 </summary>
        // BUGBUG : On Bidi OS (hebrew arabic) left > right
        public bool IsEmpty => left >= right || top >= bottom;

        /// <summary> Return a user friendly representation of this struct </summary>
        public override string ToString() => this == Empty ? "RECT {Empty}" : $"RECT {{ left : {left} / top : {top} / right : {right} / bottom : {bottom} }}";

        /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
        public override bool Equals(object obj) => obj is Rect && this == (RECT)obj;

        /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
        public override int GetHashCode()
        {
            var hash = left.GetHashCode();
            hash = (hash * 397) ^ top.GetHashCode();
            hash = (hash * 397) ^ right.GetHashCode();
            hash = (hash * 397) ^ bottom.GetHashCode();
            return hash;
        }


        /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
        public static bool operator ==(RECT rect1, RECT rect2) => rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom;

        /// <summary> Determine if 2 RECT are different(deep compare)</summary>
        public static bool operator !=(RECT rect1, RECT rect2) => !(rect1 == rect2);    
    }
}