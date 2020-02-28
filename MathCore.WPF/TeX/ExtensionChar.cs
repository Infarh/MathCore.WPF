// Extension character that contains character information for each of its parts.
namespace MathCore.WPF.TeX
{
    internal class ExtensionChar
    {
        public ExtensionChar(CharInfo top, CharInfo middle, CharInfo bottom, CharInfo repeat)
        {
            Top = top;
            Middle = middle;
            Repeat = repeat;
            Bottom = bottom;
        }

        public CharInfo Top
        {
            get;
            private set;
        }

        public CharInfo Middle
        {
            get;
            private set;
        }

        public CharInfo Bottom
        {
            get;
            private set;
        }

        public CharInfo Repeat
        {
            get;
            private set;
        }
    }
}