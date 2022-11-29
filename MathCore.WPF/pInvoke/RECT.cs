using System.Runtime.InteropServices;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.pInvoke;

[StructLayout(LayoutKind.Sequential, Pack = 0), System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "InconsistentNaming")]
public struct Rect : IEquatable<Rect>
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public static readonly Rect Empty;

    public int Width => Math.Abs(Right - Left);

    public int Height => Bottom - Top;

    public Rect(int left, int top, int right, int bottom)
    {
        Left   = left;
        Top    = top;
        Right  = right;
        Bottom = bottom;
    }

    public Rect(Rect rcSrc)
    {
        Left   = rcSrc.Left;
        Top    = rcSrc.Top;
        Right  = rcSrc.Right;
        Bottom = rcSrc.Bottom;
    }

    public bool IsEmpty => Left >= Right || Top >= Bottom;

    /// <summary> Return a user friendly representation of this struct </summary>
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