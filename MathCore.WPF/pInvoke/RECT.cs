using System.Runtime.InteropServices;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.pInvoke;

[StructLayout(LayoutKind.Sequential, Pack = 0), System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "InconsistentNaming")]
public struct Rect(int left, int top, int right, int bottom) : IEquatable<Rect>
{
    public int Left = left;
    public int Top = top;
    public int Right = right;
    public int Bottom = bottom;

    public static readonly Rect Empty;

    public int Width => Math.Abs(Right - Left);

    public int Height => Bottom - Top;

    public Rect(Rect rcSrc) : this(rcSrc.Left, rcSrc.Top, rcSrc.Right, rcSrc.Bottom)
    {
    }

    public bool IsEmpty => Left >= Right || Top >= Bottom;

    /// <summary> Return a user-friendly representation of this struct </summary>
    public override string ToString() => Equals(Empty) ? "RECT {Empty}" : $"RECT {{ left : {Left} / top : {Top} / right : {Right} / bottom : {Bottom} }}";

    public bool Equals(Rect other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;

    public override bool Equals(object? obj) => obj is Rect other && Equals(other);

    public override int GetHashCode() => HashBuilder.New(Left)
       .Append(Top)
       .Append(Right)
       .Append(Bottom);

    public static bool operator ==(Rect left, Rect right) => left.Equals(right);

    public static bool operator !=(Rect left, Rect right) => !left.Equals(right);
}