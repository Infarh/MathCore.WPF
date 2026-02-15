using System.Windows.Media;

namespace MathCore.WPF.TeX;

/// <summary>
/// Бокс, содержащий вертикальный стек дочерних боксов
/// </summary>
/// <remarks>
/// VerticalBox используется для размещения боксов в вертикальном направлении.
/// Поддерживает выравнивание (сверху, по центру, снизу).
/// </remarks>
internal class VerticalBox : Box
{
    /// <summary>Самая левая позиция дочерних боксов</summary>
    private double _LeftMostPos = double.MaxValue;

    /// <summary>Самая правая позиция дочерних боксов</summary>
    private double _RightMostPos = double.MinValue;

    /// <summary>
    /// Инициализирует новый вертикальный бокс с одним элементом и заданным выравниванием
    /// </summary>
    /// <param name="box">Элемент для размещения</param>
    /// <param name="rest">Дополнительное пространство для выравнивания</param>
    /// <param name="alignment">Выравнивание (Center, Top, Bottom)</param>
    public VerticalBox(Box box, double rest, TexAlignment alignment)
    {
        Add(box);
        switch(alignment)
        {
            case TexAlignment.Center:
                var strut_box = new StrutBox(0, rest / 2, 0, 0);
                base.Add(0, strut_box);
                Height += rest / 2;
                Depth  += rest / 2;
                base.Add(strut_box);
                break;
            case TexAlignment.Top:
                Depth += rest;
                base.Add(new StrutBox(0, rest, 0, 0));
                break;
            case TexAlignment.Bottom:
                Height += rest;
                base.Add(0, new StrutBox(0, rest, 0, 0));
                break;
        }
    }

    /// <summary>Инициализирует новый пустой вертикальный бокс</summary>
    public VerticalBox() { }

    /// <summary>
    /// Добавляет дочерний бокс и обновляет размеры контейнера
    /// </summary>
    /// <param name="box">Бокс для добавления</param>
    public override void Add(Box box)
    {
        base.Add(box);

        if(Children.Count == 1)
        {
            Height = box.Height;
            Depth  = box.Depth;
        }
        else
            Depth += box.Height + box.Depth;
        RecalculateWidth(box);
    }

    /// <summary>
    /// Добавляет дочерний бокс по указанной позиции и обновляет размеры контейнера
    /// </summary>
    /// <param name="position">Позиция для добавления бокса</param>
    /// <param name="box">Бокс для добавления</param>
    public override void Add(int position, Box box)
    {
        base.Add(position, box);

        if(position == 0)
        {
            Depth  += box.Depth + Height;
            Height =  box.Height;
        }
        else
            Depth += box.Height + box.Depth;
        RecalculateWidth(box);
    }

    /// <summary>
    /// Пересчитывает ширину контейнера на основе добавленного бокса
    /// </summary>
    /// <param name="box">Добавленный бокс</param>
    private void RecalculateWidth(Box box)
    {
        _LeftMostPos  = Math.Min(_LeftMostPos, box.Shift);
        _RightMostPos = Math.Max(_RightMostPos, box.Shift + (box.Width > 0 ? box.Width : 0));
        Width        = _RightMostPos - _LeftMostPos;
    }

    /// <summary>
    /// Рисует содержимое контейнера и его дочерних элементов
    /// </summary>
    /// <param name="Context">Контекст рисования</param>
    /// <param name="scale">Масштаб рисования</param>
    /// <param name="x">Позиция X</param>
    /// <param name="y">Позиция Y</param>
    public override void Draw(DrawingContext Context, double scale, double x, double y)
    {
        base.Draw(Context, scale, x, y);

        var cur_y = y - Height;
        foreach(var child in Children)
        {
            cur_y += child.Height;
            child.Draw(Context, scale, x + child.Shift - _LeftMostPos, cur_y);
            cur_y += child.Depth;
        }
    }

    /// <summary>
    /// Получает ID шрифта для последнего добавленного элемента
    /// </summary>
    /// <returns>ID шрифта</returns>
    public override int GetLastFontId()
    {
        var font_id = TexFontUtilities.NoFontId;
        foreach(var child in Children)
        {
            font_id = child.GetLastFontId();
            if(font_id == TexFontUtilities.NoFontId)
                break;
        }
        return font_id;
    }
}