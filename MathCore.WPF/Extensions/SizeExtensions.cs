namespace System.Windows;

public static class SizeExtensions
{
    public static void Deconstruct(this Size size, out double Width, out double Height)
    {
        Width = size.Width;
        Height = size.Height;
    }
}