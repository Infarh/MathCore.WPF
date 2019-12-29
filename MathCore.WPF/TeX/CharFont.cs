namespace MathCore.WPF.TeX
{
    /// <summary>Single character together with specific font</summary>
    internal class CharFont
    {
        public char Character { get; private set; }

        public int FontId { get; private set; }

        public CharFont(char character, int fontId)
        {
            Character = character;
            FontId = fontId;
        }
    }
}
